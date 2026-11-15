from pathlib import Path
import re, shutil
root=Path(__file__).resolve().parent.parent
assets=root/'recovery-unity62/ExportedProject/Assets/Resources/shaders'
count=0
for p in assets.glob('*.shader'):
    old=p.read_text(); match=re.search(r'Shader "([^"]+)"',old)
    if not match:continue
    name=match[1]
    if not re.fullmatch(r'(Hidden/)?Unlit/(Transparent Colored|Text|Premultiplied Colored)( [123]| \(TextureClip\))?',name):continue
    backup=root/'portability/originals-shaders'/p.name
    backup.parent.mkdir(parents=True,exist_ok=True)
    if not backup.exists():shutil.copy2(p,backup)
    clips=int(name[-1]) if name[-1].isdigit() else 0
    textureclip='TextureClip' in name
    text='/Text' in name
    premul='Premultiplied' in name
    declarations='\n'.join(f'float4 _ClipRange{i}, _ClipArgs{i};' for i in range(max(clips, int(textureclip))))
    clipping=''
    for i in range(clips):
        clipping+=f'''float2 r{i} = float2(input.local.x * _ClipArgs{i}.w - input.local.y * _ClipArgs{i}.z, input.local.x * _ClipArgs{i}.z + input.local.y * _ClipArgs{i}.w);
        float2 edge{i} = (1.0 - abs(r{i} * _ClipRange{i}.zw + _ClipRange{i}.xy)) * _ClipArgs{i}.xy;
        color.a *= saturate(min(edge{i}.x, edge{i}.y));\n'''
    if textureclip:clipping+='color.a *= tex2D(_ClipTex, (input.local * _ClipRange0.zw + _ClipRange0.xy) * 0.5 + 0.5).a;'
    sample='fixed4 color = fixed4(input.color.rgb, input.color.a * tex2D(_MainTex, input.uv).a);' if text else 'fixed4 color = tex2D(_MainTex, input.uv) * input.color;'
    p.write_text('''Shader "'''+name+'''" {
Properties { _MainTex ("Texture", 2D) = "white" {} _ClipTex ("Clip", 2D) = "white" {} }
SubShader {
 Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
 Cull Off Lighting Off ZWrite Off
 Blend '''+('One' if premul else 'SrcAlpha')+''' OneMinusSrcAlpha
 Pass {
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex, _ClipTex;
 '''+declarations+'''
 struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
 struct v2f { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; float2 local:TEXCOORD1; fixed4 color:COLOR; };
 v2f vert(appdata v) { v2f o; o.vertex=UnityObjectToClipPos(v.vertex); o.uv=v.uv; o.local=v.vertex.xy; o.color=v.color; return o; }
 fixed4 frag(v2f input):SV_Target {
 '''+sample+'\n'+clipping+('\ncolor.rgb *= input.color.a;' if premul else '')+'''
 return color;
 }
 ENDCG
 }
}
}
''');count+=1
print('Replaced',count,'NGUI shader placeholders')
