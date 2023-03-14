//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.Graphs
{
    /// <summary>
    /// The graph schema defines the rules of the theme graph
    /// </summary>
    public class GraphSchema
    {
        /// <summary>
        /// Checks if a link between the two nodes can be created
        /// </summary>
        /// <param name="output">The pin from which the link originates and goes out</param>
        /// <param name="input">The pin where the link points to</param>
        /// <returns>true, if the link is allowed, false otherwise</returns>
        public virtual bool CanCreateLink(GraphPin output, GraphPin input)
        {
            string errorMessage;
            return CanCreateLink(output, input, out errorMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output">The pin from which the link originates and goes out</param>
        /// <param name="input">The pin where the link points to</param>
        /// <param name="errorMessage"></param>
        /// <returns>true, if the link is allowed, false otherwise</returns>
        public virtual bool CanCreateLink(GraphPin output, GraphPin input, out string errorMessage)
        {
            errorMessage = "";
            if (output == null || input == null)
            {
                errorMessage = "Invalid connection";
                return false;
            }
            if (output.PinType != GraphPinType.Output || input.PinType != GraphPinType.Input)
            {
                errorMessage = "Not Allowed";
                return false;
            }

            // Make sure we don't already have this connection
            foreach (var link in output.GetConntectedLinks())
            {
                if (link.Input == input)
                {
                    errorMessage = "Not Allowed: Already connected";
                    return false;
                }
            }

            return true;
        }



    }
}
