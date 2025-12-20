using System;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Infrastructure.Registration
{
    public class BranchBuilder
    {
        private readonly BranchNode _parent;
        private FsmNode _lastNodeAdded = null;

        public BranchBuilder(BranchNode parent)
        {
            // Rationale: Si por algún error de lógica interna llegamos aquí 
            // con algo que no es una rama, cortamos en seco.
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        // Validación: Solo permite clases que hereden de LeafNode
        public BranchBuilder AddLeaf<T>(string name) where T : LeafNode, new()
        {
            var leaf = new T { Id = NodeId.New(), Name = name };
            ConfigureNode(leaf);
            return this;
        }

        // Validación: Solo permite clases que hereden de BranchNode
        public BranchBuilder AddBranch<T>(string name, Action<BranchBuilder> configure = null) where T : BranchNode, new()
        {
            var branch = new T { Id = NodeId.New(), Name = name };
            ConfigureNode(branch);

            // Rationale: Las ramas son las únicas que pueden tener un bloque de configuración
            // porque son las únicas que pueden contener otros nodos.
            if (configure != null)
            {
                var subBuilder = new BranchBuilder(branch);
                configure(subBuilder);
            }
            return this;
        }

        private void ConfigureNode(FsmNode newNode)
        {
            // Rationale: Aquí garantizamos que la estructura Rama-Hoja sea íntegra.
            newNode.Parent = _parent;

            if (_parent.FirstChild == null)
            {
                _parent.FirstChild = newNode;
            }

            if (_lastNodeAdded != null)
            {
                _lastNodeAdded.NextSibling = newNode;
            }

            _lastNodeAdded = newNode;
            _parent.Children.Add(newNode);
        }
    }
}