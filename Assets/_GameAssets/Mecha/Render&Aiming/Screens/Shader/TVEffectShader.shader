// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Scanlines" {
	Properties{
		_Color("Color", Color) = (0,0,0,1)
		_LinesSize("LinesSize", Range(1,10)) = 1
	}
		SubShader{
			Tags {"IgnoreProjector" = "True" "Queue" = "Overlay"}
			Fog { Mode Off }
			Pass {
				ZTest Always
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

		#pragma vertex vert 
		#pragma fragment frag
		#include "UnityCG.cginc"

				fixed4 _Color; half _LinesSize;

				struct v2f {
					half4 pos:POSITION;
					fixed4 sPos : TEXCOORD;
				};

				v2f vert(appdata_base v) {
					v2f o; o.pos = UnityObjectToClipPos(v.vertex);
					o.sPos = ComputeScreenPos(o.pos);
					return o;
				}

				fixed4 frag(v2f i) : COLOR {
					fixed p = i.sPos.y / i.sPos.w;

					if ((int)(p*_ScreenParams.y / floor(_LinesSize)) % 2 == 0) discard;

					   return _Color;
				 }

		ENDCG
			   }

	}

}