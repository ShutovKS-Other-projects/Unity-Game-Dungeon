Shader "DungeonArchitect/Unlit/Transparent" {
 
Properties {
    _Color ("Main Color (A=Opacity)", Color) = (1,1,1,1)
}
 
Category {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
    ZWrite Off
    Cull Off
    Blend SrcAlpha OneMinusSrcAlpha
 
    SubShader {
        Offset 0, -1
	    Pass {
	    
	        GLSLPROGRAM
	        #ifdef VERTEX
	        uniform mediump vec4 _MainTex_ST;
	        void main() {
	            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
	        }
	        #endif
	       
	        #ifdef FRAGMENT
	        uniform lowp vec4 _Color;
	        void main() {
	            gl_FragColor = _Color;
	        }
	        #endif     
	        ENDGLSL
	    }
    }
   
    SubShader {Pass {
        SetTexture[_MainTex] {Combine texture * constant ConstantColor[_Color]}
    }}
}
 
}
