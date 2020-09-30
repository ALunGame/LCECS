// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LCDemo/LCWave"
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" { }
        
        _TimeSpeed("速度",Range(1,50))=50 
        
        _XOffset("X方向偏移",Range(1,50))=1.45 
        _XAdd("X倍数",Range(0.1,50))=0.5
        
        _YOffset("Y方向偏移",Range(1,50))=1.45 
        _YAdd("Y倍数",Range(0.1,50))=0.5
    } 

    SubShader 
    { 
        Pass 
        { 
            CULL Off 
             
            CGPROGRAM 
            #pragma vertex vert 
            #pragma fragment frag 
            #include "UnityCG.cginc" 
            #include "AutoLight.cginc" 
              
            float4 _Color; 
            sampler2D _MainTex; 
            
            float _TimeSpeed; 
            
            float _XOffset; 
            float _XAdd; 
            
            float _YOffset; 
            float _YAdd; 

            // vertex input: position, normal 
            struct appdata { 
                float4 vertex : POSITION; 
                float4 texcoord : TEXCOORD0; 
            }; 
        
            struct v2f { 
                float4 pos : POSITION; 
                float2 uv: TEXCOORD0; 
            }; 
        
            v2f vert (appdata v) { 
                v2f o; 
        
                float sinOff=v.vertex.x+v.vertex.y+v.vertex.z; 
                float t=-_Time*_TimeSpeed; 
                float fx=v.texcoord.x; 
        
                v.vertex.x+=sin(t*_XOffset+sinOff)*fx*_XAdd; 
	    	    v.vertex.y+=sin(t*_YOffset+sinOff)*fx*_YAdd; 
	    	    //v.vertex.z+=sin(t*_YOffset+sinOff)*fx*_YAdd; 
                o.pos = UnityObjectToClipPos(v.vertex); 
                o.uv = v.texcoord; 
        
                return o; 
            } 
        
            float4 frag (v2f i) : COLOR 
            { 
                half4 color = tex2D(_MainTex, i.uv); 
                return color; 
            } 

            ENDCG 

            SetTexture [_MainTex] {combine texture} 
        } 
    } 
    Fallback "VertexLit" 
}
