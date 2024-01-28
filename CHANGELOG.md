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

