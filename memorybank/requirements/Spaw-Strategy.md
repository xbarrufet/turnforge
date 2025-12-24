## Descripcio del process de Spawn
Spawn es l'operacio que permet crear entitats en el joc.
Es realitza en 4 etapes
1. Creacio del SpawnRequest
2. Transformacio del SpawnRequest en un AgentDescriptor
3. Aplicacio de la Spawn Strategy per modificar el AgentDescriptor
4. Creacio de la decision per fer l'spawn de la entitat
5. Aplicacio de la decision al Applier i creacio de la entitat

### 1. Creacio del SpawnRequest
Us de fluent SpawnRequestBuilder per crear un SpawnRequest on posem la definitionId, count, position i les propietats a sobreescriure

### 2. Transformacio del SpawnRequest en un AgentDescriptor
El command de Spawn mira la definitionId i busca si esta associada a una entitat que no sigui Prod o Agent, aixpo implicara que es una entitat personalitzada.
En quest cas, busca quin es el descriptor associat i el crear.



### 3. Aplicacion de la Spawn Strategy (veure nota-01)
Per implementar la SpawnStrategy hem de creat una clase que heredi de BaseSpawnStrategy que ja implementa la interface ISpawnStrategy
SpawnStrategy funciona a 3 nivells de jerarquia
1. Primer busca si hi ha un SpawnStrategy.Process<T>(Desciptors, GameState), on T es el tipos de descriptor que es vol processar.
2. Si no hi ha un SpawnStrategy.Process<T>(Desciptors, GameState) aplica el SpawnStrategy.Process(Descriptor, GameState) generic
3. Si no hi ha un SpawnStrategy.Process(Descriptor, GameState) aplica el SpawnStrategy.Process(Descriptor, GameState) de la BseSpawnStrategy

### 4 i 5 queden com estan


#### nota-01
Per norma general, no fa falta crear entitats que heredi de Actor, pero si vols tenir alguna propia es por fer. Aquestes classes han de hereditar de Agent o Prod i implementar. En auqestes clases pots definir ICompoenents extra
En cas de voler tenir els valors dels compoentns extra instanciats en l'Spawn, has de crear un Descriptor de la nova entitat. Aquesta tupla de Entitat-Descriptor es la que es passa per la Spanw Strategy




no m'agrada com esta el SpawStraegy ara
Diguem si aixpo es possible
BaseSpawnStrategu te un metodo general anemenat RootProcess(Desciprtors, GameState) aquest metodo es el que crida el CommandHandler
RootProcess anatlitza Desccriptor a Descriptor si tenen Un descriptor propi o generic i crida
ProcessDescriptot<T>(Descriptor, GameState) que retorna 1 decisio si no te generic crida a ProcessDescriptor(Descriptor, GameState) generic que torna 1 decision
RootProcess() junta les decisins i montar el obecte de respost.
L'usuari nomes ha de impllementar els dos ProcessDescriptor, el generic i el(s) especials
Es aixpo possible? podem tenir ProcessDesciptpor<T>(Descriptors, GameState) i ProcessDescriptor(Descriptor, GameState) ?
Alguna millora?
