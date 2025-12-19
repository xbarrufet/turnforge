shader_type canvas_item;

// Define el color del borde del círculo
uniform vec4 circle_color : source_color = vec4(1.0, 0.8, 0.0, 1.0);
// Define el grosor del borde (en porcentaje del radio)
uniform float border_thickness : hint_range(0.0, 0.5) = 0.07;
// Define la suavidad del borde para evitar el 'aliasing'
uniform float feather : hint_range(0.0, 0.1) = 0.005;

void fragment() {
    vec4 texture_color = texture(TEXTURE, UV);

    vec2 center_vec = UV - vec2(0.5);
    float dist = length(center_vec);

    float outer_radius = 0.5;
    float inner_radius = outer_radius - border_thickness;

    // --- 1. CÁLCULO DEL ALFA (Recorte circular externo) ---
    // alpha_factor va de 0.0 (dentro) a 1.0 (fuera) en el borde exterior.
    float alpha_factor = smoothstep(outer_radius - feather, outer_radius, dist);
    float final_alpha = texture_color.a * (1.0 - alpha_factor);

    if (final_alpha < 0.001) {
        discard;
    }

    // --- 2. CÁLCULO DEL FACTOR DE MEZCLA DEL BORDE (AISLAMIENTO DEL ANILLO) ---

    // a) Define la transición del INTERIOR a la TEXTURA:
    //    'texture_step' va de 0.0 (en el borde interior) a 1.0 (profundamente en la textura).
    //    Si dist es menor que inner_radius - feather, es 1.0 (pura textura).
    float texture_step = smoothstep(inner_radius - feather, inner_radius, dist);

    // b) Define la transición del BORDE al EXTERIOR (ya cubierto por alpha_factor).
    //    'ring_mix' debe ser 1.0 en la zona del anillo y 0.0 en la zona de la textura.

    // Queremos:
    // - En la textura (dist < inner_radius - feather): border_mix = 0.0
    // - En el borde (inner_radius - feather < dist < outer_radius): border_mix = 1.0

    // Usamos el 'texture_step' para saber cuándo salir del área de la textura.
    // Invertimos 'texture_step': (1.0 - texture_step) es 1.0 en el borde y 0.0 en el interior.
    float ring_mix = 1.0 - texture_step;

    // El factor de mezcla final debe ser 1.0 solo en el anillo (inner_radius a outer_radius)
    // y debe ser 0.0 cuando estamos fuera del outer_radius.

    // Multiplicamos por clip_alpha para asegurar que se desvanece suavemente en el exterior.
    // clip_alpha es 1.0 dentro del borde y 0.0 fuera.
    // ring_mix es 1.0 en el borde y 0.0 dentro.

    // El factor final es la cantidad de color de BORDE que se aplica.
    // Usamos 'clamp' para asegurar que el color del borde no se aplique si la distancia es
    // demasiado pequeña (dentro de la textura) o demasiado grande (fuera del círculo).
    float border_mix = clamp(dist - inner_radius + feather, 0.0, border_thickness);
    // Lo más sencillo y correcto para aislar la región:
    float ring_region = smoothstep(inner_radius - feather, inner_radius, dist) * (1.0 - smoothstep(outer_radius - feather, outer_radius, dist));

    // Invertimos la lógica para obtener el anillo:
    // ring_mix será 1.0 en el anillo, y 0.0 en el resto del círculo.
    float border_factor = 1.0 - smoothstep(inner_radius, inner_radius + feather, dist);

    // El factor correcto:
    // smoothstep(INNER_START, INNER_END, DIST) para saber dónde empieza la textura.
    // smoothstep(OUTER_START, OUTER_END, DIST) para saber dónde termina el borde.

    // Este es el factor que funciona correctamente:
    // Factor que es 0.0 en el interior y 1.0 en el borde.
    float blend_factor = smoothstep(inner_radius - feather, inner_radius, dist);

    // Sin embargo, si lo dejamos así, el interior del círculo seguirá siendo 1.0
    // El error principal fue usar el primer smoothstep al revés.

    // Intentemos con la corrección más simple y directa:
    // La distancia al centro debe ser mayor que el radio interior.

    // 1. Factor de color del borde: 1.0 si está en el anillo, 0.0 si está dentro.
    float is_in_ring = smoothstep(inner_radius, inner_radius + feather, dist);

    // 2. Mezcla: La textura solo debe mezclarse si is_in_ring es mayor que 0.
    // El color de la textura se mezcla con el color del borde.
    vec3 final_rgb = mix(texture_color.rgb, circle_color.rgb, is_in_ring);

    // 3. Aplique el recorte alfa final.
    COLOR = vec4(final_rgb, final_alpha);
}
