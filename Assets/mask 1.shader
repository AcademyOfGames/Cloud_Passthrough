Shader "Custom/mask"
{
    Properties
    {

    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}

        Pass
        {
            Blend Zero One
            Zwrite Off

            
        }
        
    }
    FallBack "Diffuse"
}
