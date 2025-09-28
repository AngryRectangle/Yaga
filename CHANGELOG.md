#### 0.9.0 (2024-09-28)

##### New Features
* add new fluent api for reactive UI ([8b1a19be](https://github.com/AngryRectangle/Yaga/commit/8b1a19bea81be9bae4f5918398c424748d1733bc))

##### Tests
* add for new fluent api ([4b97d02a](https://github.com/AngryRectangle/Yaga/commit/4b97d02a512fb6ff1ed4263a9eed7ad0e1b35884))

#### 0.8.0 (2024-02-05)

##### BREAKING CHANGES
* remove AcceptableView implementation from Presenter ([8a1074af](https://github.com/AngryRectangle/Yaga/commit/8a1074af59047041089dadd80ca81d29d5e4a893))
* remove AcceptableView from IPresenter ([57b5a653](https://github.com/AngryRectangle/Yaga/commit/57b5a6531a8626fe7a601216d57dfd2a8edc4ed2))
* move Bind method from static to instance in UiBootstrap.cs ([b1d10744](https://github.com/AngryRectangle/Yaga/commit/b1d1074404d0c8f1e831a849deadff8bd48650df))
* remove IPresenterWithUnspecifiedView to use much safer approach ([5961a5d9](https://github.com/AngryRectangle/Yaga/commit/5961a5d9b0d0600a9d9f017eab2b4b493a073fb0))
* refactor UiBootstrap presenters binding logic ([c7a3ddda](https://github.com/AngryRectangle/Yaga/commit/c7a3ddda5a1d5e78b7c1e81bb3f9af48facaa047))
* removed all linq-style method duplicates for OptionalObservable to avoid ambiguous references ([a31b3f52](https://github.com/AngryRectangle/Yaga/commit/a31b3f528673628885d5b28cf2370ce17f5610d6))
* rename Data in observables to Value ([016262e1](https://github.com/AngryRectangle/Yaga/commit/016262e179751c7a8a4be3661c4634d86583f7f5))
* huge framework overhaul ([00bded91](https://github.com/AngryRectangle/Yaga/commit/00bded915a4ce5167233f54541298937030834d8))

##### New Features
* add internal create methods for observable model ([5dfb1c9d](https://github.com/AngryRectangle/Yaga/commit/5dfb1c9ddb14e64fb2658dd5473dc5beef7f487c))
* add set extension method for OptionalObservable ([b167518a](https://github.com/AngryRectangle/Yaga/commit/b167518a59341c658f3c9b7104cb13626d7834a3))
* add create extension to create child views ([b16455f9](https://github.com/AngryRectangle/Yaga/commit/b16455f9ab81bb9c95320c00acff7d0d85e40e65))
* add WhereSelect extension for observables ([5282f0ae](https://github.com/AngryRectangle/Yaga/commit/5282f0aec1570dcd96b266ad56db2750781b7c75))

##### Fixes:
* observable was throwing exceptions after view destroying ([87248aaa](https://github.com/AngryRectangle/Yaga/commit/87248aaa60308b6a8859938ddea5e78ad9419347))
* add null check for Subscriptions.Add ([3e8a8f70](https://github.com/AngryRectangle/Yaga/commit/3e8a8f70dcd64210757480da4ec146493d53ff5d))
* set method for observable subscription extension ([e46e28df](https://github.com/AngryRectangle/Yaga/commit/e46e28dff5e9a0ed9568c253253c59f421af3467))

##### Documentation Changes
* more xml docs for extension methods and presenters ([24dd68a4](https://github.com/AngryRectangle/Yaga/commit/24dd68a4d5b07d36a85842387f62d4fcda1433fb))
* add new info to getting started section ([d43ab27a](https://github.com/AngryRectangle/Yaga/commit/d43ab27a2a2a2965af19e6e17314d26988a5d251))
* add more info for models and presenters section ([294ca749](https://github.com/AngryRectangle/Yaga/commit/294ca749ade6b14f7fddc9092f16620ef615e3d6))
* refactor reactivity section ([ae939145](https://github.com/AngryRectangle/Yaga/commit/ae939145d9b64886023660b573d6448b08d103ec))
* add core concepts section ([da04b00c](https://github.com/AngryRectangle/Yaga/commit/da04b00ca1e7cc01d1a98d94ab83117f1ef0d50d))
* update readme to reflect big refactoring ([4f694a86](https://github.com/AngryRectangle/Yaga/commit/4f694a86d6d944b1bb265678a5b553d4fac5f8ed))
* update badges for README.md ([e36656b0](https://github.com/AngryRectangle/Yaga/commit/e36656b09cedf99d56174f36d581fe830b2dd392))

##### Continuous Integration
* add testing and coverage reporting ([e34a9d82](https://github.com/AngryRectangle/Yaga/commit/e34a9d82a84021a2504ab9b8593f042f8690df19))

##### Tests
* temporary remove for optional observable set method ([74a55c31](https://github.com/AngryRectangle/Yaga/commit/74a55c31de7da9856f79d1cd3130e6778f8b11b5))
* add test for observable Set extension method ([abe82bf2](https://github.com/AngryRectangle/Yaga/commit/abe82bf2351e408f90d248eb0b4c88379a424b69))
* fix prefabs ([eb4ebd82](https://github.com/AngryRectangle/Yaga/commit/eb4ebd8260a7137d40f6e504e13cc4817a3607f3))
* add beacon tests ([385a9a60](https://github.com/AngryRectangle/Yaga/commit/385a9a60f406b073afbb33de1b3fd9a0de6e2f7d))
* add observable tests ([955e8377](https://github.com/AngryRectangle/Yaga/commit/955e837727f90fd3f5e257e739d427f31252d51e))
* add tests for Subscriptions ([7c5bfbc0](https://github.com/AngryRectangle/Yaga/commit/7c5bfbc0e63fe411b4e966f08634dfcfdccec116))
* another attempt to fix github tests ([68e80117](https://github.com/AngryRectangle/Yaga/commit/68e8011709f77d47944a420d22c116f67fed1ebe))
* update prefab locator to fix github tests ([1395bf08](https://github.com/AngryRectangle/Yaga/commit/1395bf08d406412788958e2623892b62098ffd36))
* update for UiBootstrap ([f581659a](https://github.com/AngryRectangle/Yaga/commit/f581659ac748affdc66cf3c903f83c0e6963ccc2))

#### 0.7.0 (2024-01-28)

##### BREAKING CHANGES

* remove close/open logic to simplify ui flow ([bc15446c](https://github.com/AngryRectangle/Yaga/commit/bc15446c6deb1c37b30d8441b498f77dd40b3123))
* add readonly interfaces for observables ([1b02bab1](https://github.com/AngryRectangle/Yaga/commit/1b02bab199bcfa1d486943b57d6641c070f7a885))
* make OptionalObservable derive from Observable using Option type ([3f3ade2f](https://github.com/AngryRectangle/Yaga/commit/3f3ade2f9b8df00c0d8787594c25836137c6120d))
* rename IsDefault to HasValue ([c1c1dd96](https://github.com/AngryRectangle/Yaga/commit/c1c1dd966384aaf330a4d747c3ccc25401f6862f))
* make Observable fire event even if value isn't changed ([a6b4c1bd](https://github.com/AngryRectangle/Yaga/commit/a6b4c1bd8e4a3b42bdd317afc81f4ebcf3e14c89))
* replace bind methods with rx style methods ([04778497](https://github.com/AngryRectangle/Yaga/commit/047784976f72a88d2a69b44e7b2fe69b0b9fc026))
* rename reflector to Disposable ([4e0c39ba](https://github.com/AngryRectangle/Yaga/commit/4e0c39ba15397ec5d51f04a46c4a7ff12892c4d7))
* get rid of unused exception ([8a0b1423](https://github.com/AngryRectangle/Yaga/commit/8a0b142334f26e48782b513177daac9b01a0d875))

##### Documentation Changes

* reflect changes of OptionalObservable ([67ed7684](https://github.com/AngryRectangle/Yaga/commit/67ed76841600aaff50ee4c312aaddc9e347aef02))
* update docs to reflect changes in observables ([4b370505](https://github.com/AngryRectangle/Yaga/commit/4b3705057c71991397832d4b6c7ba3044640af83))
* exclude info about open/close stages ([c1ee141d](https://github.com/AngryRectangle/Yaga/commit/c1ee141d0d498ef5f7e28eec1228908b0f87090a))

##### New Features

* add Option type dll ([1f4154a4](https://github.com/AngryRectangle/Yaga/commit/1f4154a4c519d8b711adba69a599e91e133ef76b))
* implement System.IObservable for Yaga.Utils.Observable ([6622d8b5](https://github.com/AngryRectangle/Yaga/commit/6622d8b54590dcc567d18e9ffb45f7a53c77a2b0))
* add view subscription methods for System.IObservable ([ebe06108](https://github.com/AngryRectangle/Yaga/commit/ebe0610866576c2584085b15ee4e9d1e8d6d9918))

#### 0.6.0 (2023-03-05)

##### BREAKING CHANGES

  * replace events with beacons for observers ([3a82d61c](https://github.com/AngryRectangle/Yaga/commit/3a82d61cf37ea4a73bffd5c2d0d06d4e6749cf0f))
  * removed model argument from View.Unset ([edd8fcf8](https://github.com/AngryRectangle/Yaga/commit/edd8fcf819c9a35d27e38c4e6c87b9b128c68baf))

##### Documentation Changes

  * add table of contents ([ad2e046d](https://github.com/AngryRectangle/Yaga/commit/ad2e046d73c4e4cafd4cbe2fbb4793534c092d99))
  * translate best practices and editor binding part ([6f73a006](https://github.com/AngryRectangle/Yaga/commit/6f73a006e34c54559bf737bc7e89b1bce1498fb3))
  * update introduction ([bb839c5a](https://github.com/AngryRectangle/Yaga/commit/bb839c5a335cff17fe1a00def6be2a4f8a317ef7))

##### New Features

  * move instantiation logic to view ([6ede35d5](https://github.com/AngryRectangle/Yaga/commit/6ede35d5717b0d9fa48759e71c89a990d7280b1f))
  * remove mono behaviour constraints ([894c42b1](https://github.com/AngryRectangle/Yaga/commit/894c42b1bbe75d4418bf9fbcda6edfef28cdbd35))
  * add event subscription ([a4933102](https://github.com/AngryRectangle/Yaga/commit/a4933102b4ec820cbf67dd6bc6df9277ef8c9032))
  * add presenter interface checking on binding ([b19e8c0d](https://github.com/AngryRectangle/Yaga/commit/b19e8c0de007aa47b749e5284f05c2de0985fa8e))
  * add IPresenterWithUnspecifiedView to avoid c# generics problem with Set method ([cb7ba3cf](https://github.com/AngryRectangle/Yaga/commit/cb7ba3cf24da47f91a0bf6f4d9446f047f193ec8))
  * add canvas destroying after view destroying ([24d1ac14](https://github.com/AngryRectangle/Yaga/commit/cb7ba3cf24da47f91a0bf6f4d9446f047f193ec8))

##### Tests

  * add tests for observable array ([2f76d939](https://github.com/AngryRectangle/Yaga/commit/2f76d93965a3bde562c8e4bea074770ad54578b0))
  * add child model unset test ([45ab7363](https://github.com/AngryRectangle/Yaga/commit/45ab7363a9c1fa31ea0d735821241833a93fb0ca))
  * add tests on model property for View ([2dd84e51](https://github.com/AngryRectangle/Yaga/commit/2dd84e51345ccf917f5b1b7d03fd8515bf331665))
  * add null event checking for SubscriptionTest.cs ([369eb7ec](https://github.com/AngryRectangle/Yaga/commit/369eb7eceed9dcdb19b91165afa07d1527409a64))

#### 0.5.0 (2022-06-11)

##### BREAKING CHANGES

*  move observable binding logic to other classes ([3e164702](https://github.com/AngryRectangle/Yaga/commit/3e164702c15358335e0c856872ea0ad6d6234fef))

##### Continuous Integration

*  add code coverage report ([9a0715d9](https://github.com/AngryRectangle/Yaga/commit/9a0715d9100b465d39f42f9676a190ef497b565e))

##### Documentation Changes

*  actual coverage badge fix ([c6d58dcb](https://github.com/AngryRectangle/Yaga/commit/c6d58dcb9c0a6d0626b500c8204c73fc4202c3f7))
*  attempt to fix coverage badge ([d4810e4f](https://github.com/AngryRectangle/Yaga/commit/d4810e4fbccc0624ff8cd4f11357d87f076cd7bc))
*  update observable docs corresponding to changes ([dd46e4de](https://github.com/AngryRectangle/Yaga/commit/dd46e4deee3b80286d03447918e668158d806cdc))
*  add sections about data structures for observer pattern ([470109be](https://github.com/AngryRectangle/Yaga/commit/470109bed6bfb9c69886bec3eb0929f635f6b6b2))
*  add presenters and models section ([77b6cee6](https://github.com/AngryRectangle/Yaga/commit/77b6cee68b10c6fb3681f453a760c2b0c64634f7))
*  add view section ([bf86aa86](https://github.com/AngryRectangle/Yaga/commit/bf86aa86d494361fd16284e63bb82a87f6afbca2))
*  add initialization info to readme ([70a3b55e](https://github.com/AngryRectangle/Yaga/commit/70a3b55e825f69b7b7980dd77546d36cdb36ca45))
*  type in readme ([c0083339](https://github.com/AngryRectangle/Yaga/commit/c0083339dd18c36717a4e081218a7eba0e81bc89))
*  add getting started part ([d0632537](https://github.com/AngryRectangle/Yaga/commit/d06325378f1772e6e2f68b74ee21636e04ef2a63))

##### New Features

*  add dispose check for Reflector.cs ([6da9b93a](https://github.com/AngryRectangle/Yaga/commit/6da9b93a984c75736c302d6e4830a8a8f444c29c))
*  add empty data check to OptionalObservable.cs ([b79d5f27](https://github.com/AngryRectangle/Yaga/commit/b79d5f27eb57e5bbffd652a1b088b923cdd463e3))
*  add '+' operator subscription for observable ([cb1d1cd3](https://github.com/AngryRectangle/Yaga/commit/cb1d1cd3a03f56eff060cb27ba8fcada8d93edf1))
*  add exception on access of unsetted model ([b39a116f](https://github.com/AngryRectangle/Yaga/commit/b39a116f1e9e5f4065f15c2881255e5d6581e703))
*  add set/unset methods for view ([498a57eb](https://github.com/AngryRectangle/Yaga/commit/498a57eba061a2e6b355a46fcc5bb56f0fb319ed))

##### Tests

*  add tests on documentation examples ([9e04031f](https://github.com/AngryRectangle/Yaga/commit/9e04031f8ee27e2bdd18664c0b78ca8f06771720))
*  add test for example from getting started ([bff92fe5](https://github.com/AngryRectangle/Yaga/commit/bff92fe51df9b8f45344d18b3212215f1ef23581))
*  Add UiBootstrap testing ([c285272e](https://github.com/AngryRectangle/Yaga/commit/c285272e08cad53d72d85ccb4066816eef2138c4))

#### 0.4.0 (2022-06-05)

##### BREAKING CHANGES

*  Rename Initialize method in UiControl ([03acca6a](https://github.com/AngryRectangle/Yaga/commit/03acca6a51cad8858d6e9ab40e039620b03a996f))

##### Documentation Changes

*  add info about Initialize exception to UiBootstrap.cs docs ([4b023d89](https://github.com/AngryRectangle/Yaga/commit/4b023d895a9671b5b0a62c1d948807d561c127bd))

##### New Features

*  add model unset on scene reload or game object destroying ([948eff09](https://github.com/AngryRectangle/Yaga/commit/948eff0990c5965b00e9fc52c6dd953ded590dbe))

##### Tests

*  add modeless view testing ([910bf30b](https://github.com/AngryRectangle/Yaga/commit/910bf30bb6d7de8c5bc6d5dec30c3251cb674182))

#### 0.3.0 (2022-05-29)

##### Continuous Integration

*  update package version ([ed250db0](https://github.com/AngryRectangle/Yaga/commit/ed250db0102ae6c6d5cb6e93bdfd8fe94fb03ed8))

##### Documentation Changes

*  add breaking changes notification ([55fb46fe](https://github.com/AngryRectangle/Yaga/commit/55fb46fe4b3c4edde4682d77b2cfe0a6b7994916))
*  add installation guide ([7cf67067](https://github.com/AngryRectangle/Yaga/commit/7cf6706761c009e7a399d556b293f3a92929171a))
*  add docs to buttons subscriptions ([b5d14e72](https://github.com/AngryRectangle/Yaga/commit/b5d14e72a0acfddc9fe1f8286d5d37b7f10a7902))

##### New Features

*  add chaining binding for int to string ([1ba10927](https://github.com/AngryRectangle/Yaga/commit/1ba109274dee6ba0d20a98cada4cdd027a3dd318))
*  clear uber logging ([fe950911](https://github.com/AngryRectangle/Yaga/commit/fe9509111a675d466d04a43c2242f46730f7a065))
*  add clearing presenter method ([a6cd5c30](https://github.com/AngryRectangle/Yaga/commit/a6cd5c30148bc2cf58c2c52cdcd4893696894d00))
*  add view with no model ([187525c8](https://github.com/AngryRectangle/Yaga/commit/187525c85ebb673ac4d49f93b54a73e1888e0c93))
*  add view subscription on unity ui button ([24d3a60e](https://github.com/AngryRectangle/Yaga/commit/24d3a60ec6fc7bba16f2c8b40dff5150e6e055a7))
*  array observable array as enumerable ([2875995c](https://github.com/AngryRectangle/Yaga/commit/2875995c8caba867f23b7d696be9ce0c2049b4b4))

