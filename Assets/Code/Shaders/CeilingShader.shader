Shader "Custom/CeilingShader"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Side("Side", 2D) = "white" {}
		_Top("Top", 2D) = "white" {}
		_Bottom("Bottom", 2D) = "white" {}
		_SideScale("Side Scale", Float) = 2
		_TopScale("Top Scale", Float) = 2
		_BottomScale ("Bottom Scale", Float) = 2
	}

		SubShader{
			Tags {	"RenderType" = "Opaque"
					"Queue" = "Geometry"
					"IgnoreProjector" = "False"
					"RenderType" = "Opaque"}

			CGPROGRAM
			#pragma surface surf Standard

			sampler2D _MainTex;

			sampler2D _Side, _Top, _Bottom;
			float _SideScale;
			float _TopScale;
			float _BottomScale;

			struct Input {
				float3 worldNormal;
				float3 worldPos;
				float2 uv_MainTex;
			};

			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o) {

				if (abs(IN.worldNormal.x) > 0.5)
				{
					o.Albedo = tex2D(_MainTex, IN.worldPos.zy);
				}
				if (abs(IN.worldNormal.y) > 0.5)
				{
					o.Albedo = tex2D(_MainTex, IN.worldPos.xz);
				}
				if (abs(IN.worldNormal.z) > 0.5)
				{
					o.Albedo = tex2D(_MainTex, IN.worldPos.xy);
				}

				float2 flippedUVs = IN.uv_MainTex;
				flippedUVs.x = flippedUVs.x;
				flippedUVs.y = 1.0 - flippedUVs.y;
				fixed4 c = tex2D(_MainTex, flippedUVs) * _Color;

				o.Emission = o.Albedo;

				float3 projNormal = saturate(pow(IN.worldNormal * 1.4, 4));

				// SIDE X
				float3 x = tex2D(_Side, frac(IN.worldPos.zy * _SideScale)) * abs(IN.worldNormal.x);

				// TOP / BOTTOM
				float3 y = 0;
				if (IN.worldNormal.y > 0) {
					y = tex2D(_Top, frac(IN.worldPos.zx * _TopScale)) * abs(IN.worldNormal.y);
				}
				else {
					y = tex2D(_Bottom, frac(IN.worldPos.zx * _BottomScale)) * abs(IN.worldNormal.y);
				}

				// SIDE Z	
				float3 z = tex2D(_Side, frac(IN.worldPos.xy * _SideScale)) * abs(IN.worldNormal.z);

				o.Albedo = z;
				o.Albedo = lerp(o.Albedo, x, projNormal.x);
				o.Albedo = lerp(o.Albedo, y, projNormal.y);

			}

			ENDCG
	}
		FallBack "Diffuse"
}
