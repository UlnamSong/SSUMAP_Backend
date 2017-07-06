**숭실대 캠퍼스맵 SSUMAP 간단 API 문서**
======
**장소 목록 API**
------
* Scheme: `http(s)`
* Host: `ssumap-service.azurewebsites.net`
* API Root: `/api/spots`
* ex: `POST ssumap-service.azurewebsites.net/api/spots HTTP/1.1`

**Request**
* Query String : 
    * page : 페이징 번호 (기본 : 0)
    * take : 가져올 데이터 개수 (기본 : 30)
 

```JSON
[{
    "Id" : 1,
    "Name" : "Name of Building",
    "CategoryIndex" : 1,
    "Address" : "Seoul ~ blabla",
    "Description" : "Initial Computer Science in Korea",
    "Latitude" : 37.00001,
    "Longitude" : 127.00001
  },
  {
    "Id" : 2,
    "Name" : "Name of Building",
    "CategoryIndex" : 1,
    "Address" : "Seoul ~ blabla",
    "Description" : "Initial Computer Science in Korea",
    "Latitude" : 37.00001,
    "Longitude" : 127.00001
  },...
]
```
