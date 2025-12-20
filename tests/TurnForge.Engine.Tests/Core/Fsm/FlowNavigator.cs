using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm
{
    public class FlowNavigator
    {
        private readonly FsmNode _root;
        private readonly Dictionary<NodeId, FsmNode> _nodesById;

        public FlowNavigator(FsmNode root)
        {
            _root = root;
            _nodesById = new Dictionary<NodeId, FsmNode>();
            FlattenTree(root);
        }

        // Rationale: Indexamos los nodos por ID para que la búsqueda inicial sea O(1)
        private void FlattenTree(FsmNode node)
        {
            _nodesById[node.Id] = node;
            if (node is BranchNode branch)
            {
                foreach (var child in branch.Children)
                {
                    FlattenTree(child);
                }
            }
        }

        public NodeId? GetNextStateId(NodeId currentId)
        {
            if (!_nodesById.TryGetValue(currentId, out var current)) return null;

            // 1. ¿Es una Rama? Si entramos en ella, vamos a su primer hijo.
            if (current is BranchNode branch && branch.FirstChild != null)
            {
                return branch.FirstChild.Id;
            }

            // 2. ¿Tiene un hermano? (Navegación horizontal)
            if (current.NextSibling != null)
            {
                return current.NextSibling.Id;
            }

            // 3. Si no tiene hijos ni hermanos, hay que subir (Burbujeo)
            return GetNextSiblingFromAncestors(current);
        }

        private NodeId? GetNextSiblingFromAncestors(FsmNode node)
        {
            var parent = node.Parent;

            // Si el padre es nulo, hemos llegado al Root y no hay más que recorrer.
            if (parent == null) return null;

            // Si el padre tiene un hermano, ese es el siguiente paso lógico.
            if (parent.NextSibling != null)
            {
                return parent.NextSibling.Id;
            }

            // Si el padre tampoco tiene hermanos, seguimos subiendo recursivamente.
            return GetNextSiblingFromAncestors(parent);
        }
    }
}