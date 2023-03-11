Shader "DungeonArchitect/MiniMap/Composite"
{
    Properties
    {
		_LayoutTex("Layout Tex", 2D) = "white" {}
		_OverlayTex("Overlay Tex", 2D) = "white" {}
		_FowTex("Fog of War Tex", 2D) = "white" {}
		_FowEnabled("Fog of War Enabled", int) = 0
		_UVTransform ("UV Transform", Vector) = (0.0, 0.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

			sampler2D _LayoutTex;
			sampler2D _OverlayTex;
			sampler2D _FowTex;

			int _FowEnabled;
			float4 _UVTransform;
			float4 _LayoutTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				float2 uv = v.uv / _UVTransform.zw - _UVTransform.xy;
                o.uv = TRANSFORM_TEX(uv, _LayoutTex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				float u = i.uv.x;
				float v = i.uv.y;
				if (u < 0 || v < 0 || u > 1 || v > 1) 
				{
					return fixed4(0, 0, 0, 0);
				}

				fixed4 background = tex2D(_LayoutTex, i.uv) * i.color;
				fixed4 overlay = tex2D(_OverlayTex, i.uv);

                fixed3 col = lerp(background.xyz, overlay.xyz, overlay.a);
				float alpha = 1;
				if (_FowEnabled != 0) {
					alpha = tex2D(_FowTex, i.uv).r;
				}
				return fixed4(col, alpha);
            }
            ENDCG
        }
    }
}
