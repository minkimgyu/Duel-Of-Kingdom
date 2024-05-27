# Winter_Portfolio_Project

![ezgif-2-1745cfa890](https://github.com/minkimgyu/Winter_Portfolio_Project/assets/48249824/cb1023ec-0d6a-41c4-9027-29aae4e79449)
</br>
플레이 영상: https://youtu.be/tywLCf7nhVU?si=j_xwbnCZY-g1Uv2o

## 프로젝트 소개
Unity를 사용하여 개발한 Clash Royale 모작 게임

## 개발 기간
23. 12 ~ 24. 03

## 인원
3명 (서버 프로그래머 1명, 클라이언트 프로그래머 2명)

## 개발 환경
* Unity (C#)

## 기능 설명

* ### 상속을 활용하여 Unit, Building 구현
  <div align="center">
    <a href="https://github.com/minkimgyu/Winter_Portfolio_Project/blob/4ed4d99131ea823d88696ea89eb4fdc2b6e46629/Winter_Portfolio_Project/Assets/Scripts/AI/Entity/Entity.cs#L15">Entity 코드 보러가기</a>
    </br>
    <a href="https://github.com/minkimgyu/Winter_Portfolio_Project/blob/86f37a56c46095dc7d33d6202c4ad793d9856898/Winter_Portfolio_Project/Assets/Scripts/AI/Entity/Life/Building/Building.cs#L252">Building 코드 보러가기</a>
    </br>
    <a href="https://github.com/minkimgyu/Winter_Portfolio_Project/blob/c975441a8055f5e664d597710e416eef119e1bea/Winter_Portfolio_Project/Assets/Scripts/AI/Entity/Life/Unit/Unit.cs#L18">Unit 코드 보러가기</a>
  </div>
  
  <div align="center">
    </br>
    Entity, IDamagable, ITarget을 상속 받아서 Life 클래스를 구현했습니다. 
    </br>
    </br>
    Unit, Building의 Base Class가 되는 LifeAI를 구현했습니다.
  </div>
* ### FSM과 Behavior Tree를 활용하여 AI 시스템 구축
  <div align="center">
    <a href="https://github.com/minkimgyu/Winter_Portfolio_Project/blob/c975441a8055f5e664d597710e416eef119e1bea/Winter_Portfolio_Project/Assets/Scripts/AI/Entity/Life/Unit/Unit.cs#L99">AttackUnit 코드 보러가기</a>
    </br>
    <a href="https://github.com/minkimgyu/Winter_Portfolio_Project/blob/c975441a8055f5e664d597710e416eef119e1bea/Winter_Portfolio_Project/Assets/Scripts/AI/Entity/Life/Building/Building.cs#L54C27-L54C42">AttackBuilding 코드 보러가기</a>
    </br>
    <a href="https://github.com/minkimgyu/Winter_Portfolio_Project/blob/c975441a8055f5e664d597710e416eef119e1bea/Winter_Portfolio_Project/Assets/Scripts/AI/Entity/Life/Building/Building.cs#L172">LiveOutSpawnBuilding 코드 보러가기</a>
  </div>
  
  <div align="center">
    </br>
    AttackUnit에 Behavior Tree를 구현하여 공격 기능을 구현했고 공격 유닛의 Base Class를 제작했습니다.
    </br>
    </br>
    AttackBuilding에 Behavior Tree를 구현하여 공격 기능을 구현했고 공격 타워의 Base Class를 제작했습니다.
    </br>
    </br>
    LiveOutSpawnBuilding에 Behavior Tree를 구현하여 생성 기능을 구현했고 생성 타워의 Base Class를 제작했습니다.
  </div>

* ### FSM을 사용하여 GridController 구현

  <div align="center">
    <a href="https://github.com/minkimgyu/Winter_Portfolio_Project/blob/9698e9124fb9c4698ac4604625f451922181f8bb/Winter_Portfolio_Project/Assets/Scripts/AI/Grid/GridController.cs#L12">GridController 코드 보러가기</a>
  </div>
  </br>
  <div align="center">
    <img src="https://github.com/minkimgyu/Winter_Portfolio_Project/assets/48249824/902c6289-c666-4228-a484-86473a3aa128" width="30%" height="30%"/>
    <img src="https://github.com/minkimgyu/Winter_Portfolio_Project/assets/48249824/8c24f618-dc03-49e5-9dfb-f50dcef90d6d" width="30%" height="30%"/>
  </div>

  <div align="center">
    </br>
    FSM을 통해 GridController의 Select, Plant 기능을 구현했습니다.
  </div>

* ### A* 알고리즘을 활용한 길 찾기 알고리즘 구현 및 적용
  <div align="center">
    <img src="https://github.com/minkimgyu/Winter_Portfolio_Project/assets/48249824/abac3128-2fa6-4cac-87ff-399256cd6b0a" width="60%" height="60%"/>
  </div>

  <div align="center">
    </br>
    길찾기 알고리즘을 적용했습니다.
  </div>
