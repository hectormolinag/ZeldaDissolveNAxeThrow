Shader "Custom/ToonRampWarp"
{
    Properties
    {
        _Color ("Color (RGB)", Color) = (1,1,1,1)
        _MainTex ("Texture (Albedo)", 2D) = "white"{}
        _RampTex ("Ramp Texture", 2D) = "white"{}
        _NormalMap ("Normal Map", 2D) = "bump"{}
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _SliderBump ("Bump Amount", Range(0,10)) = 1

        _DissolveMap ("Dissolve Map", 2D) = "white" {}
		_DissolveAmount ("DissolveAmount", Range(0,1)) = 0
		_DissolveColor ("DissolveColor", Color) = (1,1,1,1)
		_DissolveEmission ("DissolveEmission", Range(0,1)) = 1
		_DissolveWidth ("DissolveWidth", Range(0,0.1)) = 0.05

		_ShowEmission ("See Emission Map", Range(0,12)) = 0

		_RimPow ("Rim Pow", Range(0.3, 8.0)) = 3

    }
    SubShader
    {
        Tags { "Queue"="Geometry" }
      
        CGPROGRAM

        #pragma surface surf ToonShader

        sampler2D _MainTex;
        sampler2D _RampTex;
        sampler2D _NormalMap;
        sampler2D _EmissionMap;
        float4 _Color;
        half _SliderBump;
        float _ShowEmission;

        sampler2D _DissolveMap;
        half _DissolveAmount;
		half _DissolveEmission;
		half _DissolveWidth;
		fixed4 _DissolveColor;

		float _RimPow;

        float4 LightingToonShader(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            float diff = dot (s.Normal, lightDir);
            float h = diff * 0.5 + 0.5;
            float2 rh = h;
            float3 ramp = tex2D(_RampTex, rh).rgb;

            float4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (ramp);
            c.a = s.Alpha;
            return c;
        }

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_EmissionMap;
            float3 viewDir;

            float2 uv_DissolveMap;
            
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
        	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
        	fixed4 mask = tex2D (_DissolveMap, IN.uv_DissolveMap);

        	if(mask.r < _DissolveAmount)
				discard;

			o.Albedo = c.rgb;

			if(mask.r < _DissolveAmount + _DissolveWidth) {
				o.Albedo = _DissolveColor;
				o.Emission = _DissolveColor * _DissolveEmission;
			}

            o.Alpha = c.a;

            o.Normal = UnpackNormal(tex2D (_NormalMap, IN.uv_NormalMap));
            o.Normal *= float3(_SliderBump, _SliderBump, 1);

           half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));

            o.Emission = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _Color + (tex2D(_DissolveMap, IN.uv_DissolveMap).rgb * _ShowEmission * _Color);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
