Shader "Unlit/Swish"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{

		Tags {"Queue" = "Transparent"  "RenderType"="Transparent" }
		ZWrite Off
		  Cull Off
		  Blend SrcAlpha OneMinusSrcAlpha
		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 uv2 : TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = float4(v.uv, 0 ,0);
				float4 vertData = tex2Dlod(_MainTex, float4(o.uv, 0, 0)).rrrr;
				v.vertex.xyz -= v.normal * 0.02;
				v.vertex.xyz += vertData * v.normal * 0.1;
				o.vertex = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 tex = tex2D(_MainTex, i.uv2);
				fixed4 col = tex2D(_MainTex, i.uv).r * _Color * tex.g * tex.b;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
