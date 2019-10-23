Shader "Tests/2D/TestShader"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		coso ("Scroll Speed", Float) = 1
		escala ("Escala", Float) = 1
	}
	SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				struct appdata{
					float4 position : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f{
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float coso;
				float escala;

				v2f vert (appdata v){
					v2f o;
					o.position = UnityObjectToClipPos(v.position);
					o.uv = (v.uv + float2(_Time.y * coso,0))*escala;
					return o;
				}

				fixed4 frag (v2f i) : SV_Target{
					float4 col1 = tex2D(_MainTex, i.uv);
					float4 col2 = float4((sin(_Time.y+3.14159/3)+1)/2,(sin(_Time.y+3.14159*2/3)+1)/2,(sin(_Time.y+3.14159)+1)/2,0.5);
					return 1-((1-col1)*(1-col2));
				}
				ENDCG
			}
		}
}