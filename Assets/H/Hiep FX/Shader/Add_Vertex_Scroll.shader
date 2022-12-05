// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VFX/Add_UV_Scroll"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		scrollX ("Speed X", Float) = 1
		scrollY ("Speed Y", Float) = 1
		
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
        LOD 100
		ZWrite Off
		Cull Off
		Blend SrcAlpha One
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v_in
            {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			
            };

            struct f_in
            {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float scrollX;
			float scrollY;


            f_in vert (v_in v)
            {
				f_in o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord*_MainTex_ST.xy + _MainTex_ST.zw;
				o.texcoord.x += scrollX * _Time.y;
				o.texcoord.y += scrollY * _Time.y;
				return o;
			
            }

            fixed4 frag (f_in i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				return col;
            }
            ENDCG
        }
    }
}
