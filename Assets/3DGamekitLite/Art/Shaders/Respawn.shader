

Shader "Custom/Respawn" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		_Normal ("Normal", 2D) = "bump" {}
		_MetallicSmooth ("Metallic (RGB) Smooth (A)", 2D) = "white" {}
		[HDR]_EdgeColor1 ("Edge Color", Color) = (1,1,1,1)
		[HDR]_EdgeColor2 ("Edge Color", Color) = (1,1,1,1)
		_Noise ("Noise", 2D) = "white" {}

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Cutoff ("Cutoff", Range(0.01,1)) = 1.0
		_Cutoff2 ("Cutoff2", Range(0.0,2.0)) = 1.0
		_EdgeSizeBot ("EdgeSizeBot", Range(0,1)) = 0.2
		_EdgeSizeTop ("EdgeSizeTop", Range(0,1)) = 0.2
		_BoundsY ("BoundsY", Float) = 1.0
		
		_bounds ("bounds", Vector) =  (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
		Cull Off

		LOD 200
		
		CGPROGRAM

		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		#pragma multi_compile __ _USE_GRADIENT_ON

		sampler2D _MainTex;
		sampler2D _Noise;
		sampler2D _Gradient;
		sampler2D _Normal;
		sampler2D _MetallicSmooth;

		struct Input {
			float2 uv_Noise;
			float2 uv_MainTex;
			fixed4 color : COLOR0;
			float3 worldPos;
		};


		half _Glossiness, _Metallic, _Cutoff, _Cutoff2, _EdgeSizeBot, _EdgeSizeTop, _NoiseStrength, _BoundsY;
		half4 _Color, _EdgeColor1, _EdgeColor2;
		float4 _bounds;


		void vert (inout appdata_full v, out Input o) {
        	UNITY_INITIALIZE_OUTPUT(Input,o);

        	float4 worldPos = mul( unity_ObjectToWorld, v.vertex);
        	float3 pos =  worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
        	float yBound = _bounds.y + 0.2;
        	float temp = step(_Cutoff*1-(1/2), pos.y);

      		


    	  }

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {


			half4 MetallicSmooth = tex2D (_MetallicSmooth, IN.uv_MainTex);
			float3 pos = IN.worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;


			float2 noiseUV1 = float2(pos.y*2, pos.z*0.2);
			float2 noiseUV2 = float2(pos.y*4, pos.x*0.2);

			noiseUV1.x += _Cutoff*2;
			noiseUV2.x += _Cutoff;	

			float yBound = _bounds.y + 0.2;

			half a = tex2D (_Noise, noiseUV1).x;
			half b = tex2D (_Noise, noiseUV2).x;

			half topNoise = a*b*2;
			half botNoise =  tex2D (_Noise, float2(pos.y*6, 0)).x;

			float cutoff = _Cutoff*yBound-(yBound/2);


			half Edge = step(cutoff, pos.y);
			half EdgeBot = smoothstep(cutoff - _EdgeSizeBot, cutoff, pos.y);
			half EdgeTop = smoothstep(cutoff + _EdgeSizeTop, cutoff, pos.y);

			half3 glowBot = _EdgeColor1 * EdgeBot * (1-Edge);
			half3 glowTop = lerp(_EdgeColor2, _EdgeColor1, EdgeTop) * Edge;

			o.Albedo = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Emission = (glowBot * botNoise) + (glowBot * 0.5) + glowTop;
			o.Normal = UnpackNormal (tex2D (_Normal, IN.uv_MainTex));
			o.Metallic = MetallicSmooth.r * _Metallic * (1-EdgeBot);
			o.Smoothness = MetallicSmooth.a * _Glossiness * (1-EdgeBot);
			clip(1-Edge + (topNoise*EdgeTop) - _Cutoff - _Cutoff2);

		}
		ENDCG
	}
	FallBack "Diffuse"
}
