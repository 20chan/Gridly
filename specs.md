# Specs

## Serialization

회로는 직렬화 될 수 있다.

직렬화 된 파트들은 고유한 레퍼런스 아이디를 가지며, 이는 직렬화된 전체 회로에서만 사용되고 런타임 ID와는 관계가 없다.

파트는 JSON 포맷으로 직렬화 할 수 있다.

```json
{
    "ID": 0,
    "Type": 1,
    "Activated": true,
    "Position": {
        "x": 0,
        "y": 0
    },
    "Connecting": [1, 2],
    "Inputs": [3, 4],
    "Outputs": [5, 6],
    "Parts": [
        {}, {}, {}
    ]
}
```

- `ID` : 레퍼런스 ID
- `Type` : `0`이면 뉴런, `1`이면 일반 회로, `2`이면 빌트인 회로
- `Activated` (뉴런만) : 활성화되어있는가
- `Position` : 위치 벡터
- `Connecting` : 연결된 파트들의 ID 배열
- `Inputs` (일반 회로만) : 입력 뉴런들의 ID
- `Outputs` (일반 회로만) : 출력 뉴런들의 ID
- `Parts` (일반 회로만) : 속한 파트들의 배열
- `Character` (빌트인 회로만) : 식별자

하나의 회로를 직렬화한 전체 JSON 파일은 루트 회로의 직렬화로 나타내어진다. 루트 회로의 연결들은 비어있어야 한다.

### Example

다음과 같은 회로 구성을 생각해보자. (`△`는 회로, `□`는 뉴런, `i`는 입력, `o`는 출력 옆에 *이 붙으면 활성화)

```
전체:
i (0) --> △ (1) --> o*(2)
      \         \
       \> □ (3)  \> o (4)

1번 회로:
i*(5) <-> o (6) --> o*(7)
```

이는 다음과 같이 직렬화된다. (귀찮아서 위치는 뺐음.)

```json
{
    "ID": -1,
    "Type": 1,
    "Connecting": [],
    "Inputs": [],
    "Outputs": [],
    "Parts": [
        {
            "ID": 0,
            "Type": 0,
            "Activated": false,
            "Connecting": [1, 3]
        },
        {
            "ID": 1,
            "Type": 1,
            "Connecting": [2, 4],
            "Inputs": [5],
            "Outputs": [6, 7],
            "Parts": [
                {
                    "ID": 5,
                    "Type": 0,
                    "Activated": true,
                    "Connecting": [6]
                },
                {
                    "ID": 6,
                    "Type": 0,
                    "Depth": 1,
                    "Activated": false,
                    "Connecting": [5, 7]
                },
                {
                    "ID": 7,
                    "Type": 0,
                    "Activated": true,
                    "Connecting": []
                }
            ]
        },
        {
            "ID": 2,
            "Type": 0,
            "Activated": true,
            "Connecting": []
        },
        {
            "ID": 3,
            "Type": 0,
            "Activated": false,
            "Connecting": []
        },
        {
            "ID": 4,
            "Type": 0,
            "Activated": false,
            "Connecting": []
        }
    ]
}
```

### Stage

스테이지의 직렬화는 다음의 포맷으로 이루어진다.

```json
{
    "Parts": [

    ],
    "TestCases": [
        {
            "input": [
                [false, false, true, true],
                [true, true, false, true]
            ],
            "output": [
                [false, false, false, true]
            ]
        },
        {
            "input": [
                [true, true],
                [true, false]
            ],
            "output": [
                [true, true, false]
            ]
        }
    ]
}
```

- `Parts`: 메인 에디터에 출력될 파트들
- `TestCases`: 테스트 케이스의 리스트. 하나의 테스트 케이스는 2차원 배열의 입력/출력으로 이루어짐.
