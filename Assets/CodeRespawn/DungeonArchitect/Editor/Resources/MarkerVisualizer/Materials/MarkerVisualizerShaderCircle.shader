Shader "DungeonArchitect/Unlit/MarkerVisualizerCircle"
{
    Properties
    {
        _BodyColor ("Body Color", Color) = (0, 0.5, 1, 0.5)
        _BorderColor ("Border Color", Color) = (0, 0, 0, 0.75)
        _BorderThickness ("Border Thickness", Float) = 0.01
        _CrossThickness ("Cross Thickness", Float) = 0.01
    }
    
    SubShader
    {
        Tags {
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
        }
        LOD 100
        
        Cull off   
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _BodyColor;
            float4 _BorderColor;
            float _BorderThickness;
            float _CrossThickness;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col;
                
                float2 originUV = i.uv - float2(0.5, 0.5);
                // Check if we are inside the cross line
                float halfCrossThickness = _CrossThickness * .5;
                if (abs(originUV.x) < halfCrossThickness || abs(originUV.y) < halfCrossThickness) {
                    col = _BorderColor;
                }
                else {
                    float d = length(originUV) * 2;
                    float s = 0;

                    float thickness = clamp(_BorderThickness, 0, 1);
                    if (d < 1 - thickness) {
                        col = _BodyColor;
                    }
                    else if (d < 1) {
                        col = _BorderColor;
                    }
                    else {
                        col = float4(0, 0, 0, 0);
                    }
                }
                return col * i.color;
            }
            ENDCG
        }
    }
}
