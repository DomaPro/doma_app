Shader "Custom/RoofShader"
{
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf Standard

			sampler2D _MainTex;

			struct Input {
				float3 worldNormal;
				float3 worldPos;
				float2 uv_MainTex;
			};

			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o) {

				if (abs(IN.worldNormal.x) > 0.5)
				{
					o.Albedo = tex2D(_MainTex, IN.worldPos.yz);
				}
				if (abs(IN.worldNormal.y) > 0.5)
				{
					o.Albedo = tex2D(_MainTex, IN.worldPos.xz);
				}
				if (abs(IN.worldNormal.z) > 0.5)
				{
					o.Albedo = tex2D(_MainTex, IN.worldPos.xz);
				}

				/*float2 flippedUVs = IN.uv_MainTex;
				flippedUVs.x = flippedUVs.x;
				flippedUVs.y = 1.0 - flippedUVs.y;
				fixed4 c = tex2D(_MainTex, flippedUVs) * _Color;*/

				o.Emission = o.Albedo;

				//o.Albedo = c.rgb;
				//o.Alpha = c.a;
			}

			ENDCG
	}
		FallBack "Diffuse"
}
