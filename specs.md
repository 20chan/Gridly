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
    "Connecting": [1, 2],
    "Parts": [
        {}, {}, {}
    ],
    "Inputs": [3, 4],
    "Outputs": [5, 6]
}
```

- `ID` : 레퍼런스 ID
- `Type` : `0`이면 뉴런, `1`이면 회로
- `Activated` (뉴런만) : 활성화되어있는가
- `Connecting` : 연결된 파트들의 ID 배열
- `Parts` (회로만) : 속한 파트들의 배열
- `Inputs` (회로만) : 입력 뉴런들의 ID
- `Outputs` (회로만) : 출력 뉴런들의 ID

하나의 회로를 직렬화한 전체 JSON 파일은 파트들의 배열로 나타내어진다.

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

이는 다음과 같이 직렬화된다.

```json
[
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
        ],
        "Inputs": [5],
        "Outputs": [6, 7]
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
```