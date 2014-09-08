float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1,1,1,1);
	float AmbientIntensity = 0.75;

float4 DirectionalColor = float4(1,1,1,1);
float DirectionalIntensity = 0.50;
float3 DirectionalLightDir = float3(-1,-1,1);

 //texture ModelTexture;
//sampler2D textureSampler = sampler_state {
//	Texture = (ModelTexture);
//	MagFilter = Linear;
//	MinFilter = Linear;
//	AdressU = Clamp;
//	AdressV = Clamp;
//};

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal   : NORMAL0;
	//float2 TextureCoordinate : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Normal   : TEXCOORD0;
	//float2 TextureCoordinate : TEXCOORD1;
	
	

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.Normal = normalize(mul(input.Normal, World)); //normale wird in das welt-koordinaten-system überführt und normalisiert
	//output.TextureCoordinate = input.TextureCoordinate;


    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	    float3 normalizedLightDirection = normalize(DirectionalLightDir); //richtung wird normalisiert
		float4 diffuse = dot(normalizedLightDirection, input.Normal) * DirectionalIntensity * DirectionalColor; //winkel zwischen licht und normale wird errechnet und mit intensität des lichtes und der lichtfarbe multipliziert, wichtig: skalarprodukt
		float4 ambient = AmbientColor * AmbientIntensity; // umgebungslicht wird berechnet
		//float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
		//kombination dif. licht und umgebungslicht
		//saturate:stellt sicher, dass der eingabeparameter im bereich von 0 bis 1 ist
		return saturate(diffuse + ambient);
		//<span style = "font-weight: bold; height:12px" id="s0773d_10" class="s0773d"> saturate</span>
		//(textureColor + diffuse + ambient);
		

    
}

technique DirectionalLight
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
