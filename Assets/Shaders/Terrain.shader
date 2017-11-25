// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Terrain" {
	Properties 
	{
		_Color ("Sand Color", Color) = (1,1,1,1)
		_Direction ("Rock Direction", Vector) = (0,1,0)
		_SurfaceColor ("Sand Color", Color) = (1,1,1,1)
		_Amount ("Sand Falloff", float) = 1
		_SandLevel ("Always Sand Bellow", float) = 1

		_GrassColor ("Grass Colour", Color) = (1,1,1,1)
		_GrassDirection ("Grass Direction", Vector) = (0,1,0)
		_GrassAmount("Grass amount", float) = 0
		_HeightMin ("Grass Height Min", float) = -1
		_HeightMax ("Grass Height Max", float) = 1
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float3 worldPos;
			float2 uv_MainTex;
			float3 worldNormal;
             INTERNAL_DATA
		};

		fixed4 _Color;
		float4 _SurfaceColor;
        float4 _Direction;
		float _Amount;
		float _SandLevel;

		fixed4 _GrassColor;
		float4 _GrassDirection;
		float _GrassAmount;
		float _HeightMin;
		float _HeightMax;

		 Input vert (inout appdata_full v) 
		 {
			Input o;        
           o.worldNormal = mul(UNITY_MATRIX_IT_MV, _Direction);//convert normal to world coordinates
		   return o;
         }
		

		UNITY_INSTANCING_CBUFFER_START(Props)

		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			 if(dot(WorldNormalVector(IN, o.Normal), _Direction.xyz) >= lerp(1,-1,_Amount) || IN.worldPos.y < _SandLevel)//paint sand bellow sea/at angle
				 o.Albedo = _SurfaceColor.rgb;

			if(dot(WorldNormalVector(IN, o.Normal), _GrassDirection.xyz) >= lerp(1,-1,_GrassAmount) && IN.worldPos.y > _HeightMin && IN.worldPos.y < _HeightMax)//grass at angle and height range
				o.Albedo = _GrassColor.rgb;	

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
