# Winter_Portfolio_Project

![ezgif-2-1745cfa890](https://github.com/minkimgyu/Winter_Portfolio_Project/assets/48249824/cb1023ec-0d6a-41c4-9027-29aae4e79449)


## 프로젝트 소개
Unity를 사용하여 개발한 Clash Royale 모작 게임

## 개발 기간
23. 12 ~ 24. 03

## 인원
3명 (서버 프로그래머 1명, 클라이언트 프로그래머 2명)

## 개발 환경
* Unity (C#)

## 기능 설명

* ### 상속을 활용하여 Unit, Tower 구현
  <div align="center">
    <a href="https://github.com/minkimgyu/Winter_Portfolio_Project/blob/4ed4d99131ea823d88696ea89eb4fdc2b6e46629/Winter_Portfolio_Project/Assets/Scripts/AI/Entity/Entity.cs#L15">코드 보러가기</a>
  </div>
  
  <div align="center">
    Entity, IDamagable, ITarget을 상속 받아서 Life 클래스를 구현했습니다. 
    </br>
    </br>
    Life에 FSM을 구현하여 Unit, Tower의 Base Class가 되는 LifeAI를 구현했습니다.
  </div>
* ### FSM과 Behavior Tree를 활용하여 AI 시스템 구축

--> 링크 걸어주기
--> BT 그림 넣기

* ### FSM을 사용하여 GridController 구현

  --> 링크 걸어주기
  --> 제공하는 기능 움짤로 보여주기
  FSM을 통해 Select, Plant 기능을 구현했습니다.

* ### A* 알고리즘을 활용한 길 찾기 알고리즘 구현 및 적용

  --> 제공하는 기능 움짤로 보여주기
  --> 라인 나오는 사진 추가
