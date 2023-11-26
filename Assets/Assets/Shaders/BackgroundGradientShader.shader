Shader "Custom/GrowingLine"
{
    Properties
    {
        _PositionX ("Position X", Range(0.0, 1.0)) = 0.5
        _PositionY ("Position Y", Range(0.0, 1.0)) = 0.5
        _LineThickness("Line Thickness", Float) = 0.3
        _InitialPosition ("Initial Position", vector) =  (0.5, 0.5, 1, 1)
        _CircleRadius ("Circle Radius", Float) = 1.0
        _GrowthSpeed ("Growth Speed", Float) = 0.3
        _GameTime ("GameTime", Float) = 0.0 // New property for animation start time
                _BranchColor ("Branch Color", Color) = (1, 0, 0, 1) // Default red, change as needed

        _NumBranches ("Number of Branches", Int) = 10 
        _BranchDirection ("Branch Directions",vector) =  (0.5, 0.5,0,0)
        _BranchGrowthSpeed ("Branch Growth Speeds", Float) = 0.3
    } 

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
                  CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _PositionX;
            float _PositionY;
            float _CircleRadius;
            float _LineThickness;
            float _GrowthSpeed;
            float _AnimationOn;
            float _GameTime;

            float4 _BranchColor;
            int _NumBranches = 10;
            static const int MaxBranches = 10;
            float _BranchStartPositions[MaxBranches];
            vector _BranchDirection;

            float2 _BranchDirections[MaxBranches];
            float _BranchGrowthSpeeds[MaxBranches];

            vector _InitialPosition;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            void InitializeBranchArrays()
            {
                for (int i = 0; i < MaxBranches; ++i)
                {
                    _BranchStartPositions[i] = 0.5; // Start at middle of main line
                    _BranchDirections[i] =  _BranchDirection.xy; // Default direction
                    _BranchGrowthSpeeds[i] = 0.3; // Default growth speed
                }
            }

            fixed4 DrawLine(float2 uv, float2 start, float elapsedTime, float2 direction, float thickness, float4 color)
            {
                float2 end = start + direction * elapsedTime;
                bool isWithinLine = abs(uv.y - start.y) < thickness && uv.x >= start.x && uv.x <= end.x;
                return isWithinLine ? color : fixed4(0, 0, 0, 0);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                InitializeBranchArrays();

                float normalizedGrowthSpeed = _GrowthSpeed * 0.01; 
                float normalizedLineThickness = _LineThickness * 0.0001;
                float normalizedcircleRadius = _CircleRadius * 0.001;

                float2 center = float2(_InitialPosition.x, _InitialPosition.y); 
                float elapsedTime = _GameTime * normalizedGrowthSpeed;

                // Circle logic
                float distanceFromCenter = distance(i.uv, center); 
                bool isWithinCircle = distanceFromCenter < (normalizedcircleRadius * 0.5);
                fixed4 circleColor = isWithinCircle ? fixed4(1, 1, 1, 1) : fixed4(0, 0, 0, 0);

                // Main line logic
                fixed4 mainLineColor = DrawLine(i.uv, center, elapsedTime, float2(1, 0), normalizedLineThickness, fixed4(1, 1, 1, 1)); 

                fixed4 finalColor = max(mainLineColor, circleColor);
                for (int j = 0; j < _NumBranches; ++j)
                {
                    float branchStartX = center.x + (_BranchStartPositions[j] * normalizedGrowthSpeed);
                    float branchStartTime = _BranchStartPositions[j] / _GrowthSpeed;
                    if (_GameTime > branchStartTime)
                    {
                        float branchNormalizedGrowthSpeed = _BranchGrowthSpeeds[j] * 0.001;
                        float branchElapsedTime = (_GameTime - branchStartTime) * branchNormalizedGrowthSpeed;
                        fixed4 branchColor = DrawLine(i.uv, float2(branchStartX, 0.5), branchElapsedTime, _BranchDirection.xy, normalizedLineThickness, _BranchColor);
                        finalColor = max(finalColor, branchColor);
                    }
                }

                return finalColor;
            }


            ENDCG

        }
    }
}
