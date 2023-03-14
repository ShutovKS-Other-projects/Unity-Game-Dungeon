//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Graphs.Layouts;
using DungeonArchitect.Graphs.Layouts.Layered;
using DungeonArchitect.Graphs.Layouts.Spring;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Editors.SnapFlow
{
    public class SnapEdResultGraphEditorConfig : ScriptableObject
    {
        public GraphLayoutType layoutType;
        public GraphLayoutLayeredConfig configLayered;
        public GraphLayoutSpringConfig configSpring;

        private void OnEnable()
        {
            if (configLayered == null)
            {
                configLayered = new GraphLayoutLayeredConfig();
            }
            if (configSpring == null)
            {
                configSpring = new GraphLayoutSpringConfig();
            }
        }

        public void SaveState(KeyValueData editorData)
        {
            editorData.Set("layoutType", (int)layoutType);

            if (configLayered != null)
            {
                editorData.Set("layered.separation", configLayered.separation);
            }

            if (configSpring != null)
            {
                editorData.Set("spring.interNodeDistance", configSpring.interNodeDistance);
                editorData.Set("spring.interNodeTension", configSpring.interNodeTension);
                editorData.Set("spring.springDistance", configSpring.springDistance);
                editorData.Set("spring.springTension", configSpring.springTension);
                editorData.Set("spring.iterations", configSpring.iterations);
                editorData.Set("spring.timeStep", configSpring.timeStep);
            }
        }

        public void LoadState(KeyValueData editorData)
        {
            int layoutTypeValue = 0;
            if (editorData.GetInt("layoutType", ref layoutTypeValue))
            {
                layoutType = (GraphLayoutType)layoutTypeValue;
            }

            if (configLayered == null)
            {
                configLayered = new GraphLayoutLayeredConfig();
            }

            editorData.GetVector2("layered.separation", ref configLayered.separation);

            if (configSpring == null)
            {
                configSpring = new GraphLayoutSpringConfig();
            }

            editorData.GetFloat("spring.interNodeDistance", ref configSpring.interNodeDistance);
            editorData.GetFloat("spring.interNodeTension", ref configSpring.interNodeTension);
            editorData.GetFloat("spring.springDistance", ref configSpring.springDistance);
            editorData.GetFloat("spring.springTension", ref configSpring.springTension);
            editorData.GetInt("spring.iterations", ref configSpring.iterations);
            editorData.GetFloat("spring.timeStep", ref configSpring.timeStep);
        }
    }
}
