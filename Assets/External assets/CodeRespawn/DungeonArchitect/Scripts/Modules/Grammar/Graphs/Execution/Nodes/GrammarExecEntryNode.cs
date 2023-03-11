//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Graphs;


namespace DungeonArchitect.Grammar
{
    public class GrammarExecEntryNode : GrammarExecNodeBase
    {
        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);
            canBeDeleted = false;
            caption = "Entry";
        }
    }
}
