mergeInto(LibraryManager.library, {
	
   WelwiseGames_Init: function (successCallback, errorCallback) {
      if (wsdk != null) {
         wasmTable.get(successCallback)()
         return;
      }
      WelwiseGames.init()
         .then(function (sdk) {
            wsdk = sdk;
            wasmTable.get(successCallback)();
         })
         .catch(function (error) {
            var errorString = (error && error.message) ?
               error.message :
               "Unknown error";

            var buffer = _malloc(errorString.length + 1);
            stringToUTF8(errorString, buffer, errorString.length + 1);

            wasmTable.get(errorCallback)(buffer);
            _free(buffer);
         });
   },
   
   WelwiseGames_GetEnvirData: function () {
      try {
         var userAgent = navigator.userAgent;
         var browser = 'Other';
         if (userAgent.includes('YaBrowser') || userAgent.includes('YaSearchBrowser')) browser = 'Yandex';
         else if (userAgent.includes('Opera') || userAgent.includes('OPR')) browser = 'Opera';
         else if (userAgent.includes('Firefox')) browser = 'Firefox';
         else if (userAgent.includes('MSIE')) browser = 'IE';
         else if (userAgent.includes('Edge')) browser = 'Edge';
         else if (userAgent.includes('Chrome')) browser = 'Chrome';
         else if (userAgent.includes('Safari')) browser = 'Safari';

         const sdk = wsdk;
         const env = sdk ? sdk.Environment : null;
         const deviceType = env ? env.DeviceType.toLowerCase() : "desktop";
         const isMobile = deviceType === "mobile";
         const isTablet = deviceType === "tablet";
         const isDesktop = !isMobile && !isTablet;

         const urlParams = new URLSearchParams(window.location.search);
         const payload = urlParams.get('payload') || "";

         let jsonEnvir = {
            "language": env ? env.LanguageCode : (navigator.language || "en"),
            "browser": browser,
            "platform": navigator.platform,
            "deviceType": deviceType,
            "isMobile": isMobile,
            "isTablet": isTablet,
            "isDesktop": isDesktop,
            "payload": payload
         };

         var jsonStr = JSON.stringify(jsonEnvir);
         var buffer = _malloc(jsonStr.length + 1);
         stringToUTF8(jsonStr, buffer, jsonStr.length + 1);
         return buffer;
      } catch (e) {
         console.error('Environment Error: ', e.message);
         var errorStr = "NO_DATA";
         var buffer = _malloc(errorStr.length + 1);
         stringToUTF8(errorStr, buffer, errorStr.length + 1);
         return buffer;
      }
   },
   
   Welwise_LoadPlayerData: function (callback) {
       if (typeof wsdk === 'undefined' || !wsdk.player) {
           console.warn('Welwise player API not available');
           wasmTable.get(callback)(0);
           return;
       }
   
       wsdk.player.getData()
           .then(data => {
               const response = {
                   playerName: data.playerName || "",
                   playerGameData: data.playerGameData || []
               };
               
               const jsonData = JSON.stringify(response);
               
               const bufferSize = lengthBytesUTF8(jsonData) + 1;
               const buffer = _malloc(bufferSize);
               stringToUTF8(jsonData, buffer, bufferSize);
               
               wasmTable.get(callback)(buffer);
           })
           .catch(error => {
               console.error('LoadPlayerData error:', error);
               wasmTable.get(callback)(0);
           });
   },

   Welwise_SavePlayerData: function (playerNamePtr, gameDataPtr, callback) {
      if (typeof wsdk === 'undefined' || !wsdk.player) {
         console.warn('Welwise player API not available');
         wasmTable.get(callback)();
         return;
      }

      const playerName = UTF8ToString(playerNamePtr);
      const gameData = UTF8ToString(gameDataPtr);

      wsdk.player.setData({
            playerName: playerName,
            playerGameData: [{
               identifier: "game_save",
               value: gameData
            }]
         })
         .then(() => wasmTable.get(callback)())
         .catch(error => {
            console.error('SavePlayerData error:', error);
            wasmTable.get(callback)();
         });
   },

   Welwise_LoadMetaverseData: function (callback) {
       if (typeof wsdk === 'undefined' ||
           !wsdk.metaversePlayer ||
           !wsdk.metaversePlayer.getGameData) {
           console.warn('Welwise metaverse API not available');
           wasmTable.get(callback)(0);
           return;
       }
   
       wsdk.metaversePlayer.getGameData()
           .then(data => {
               const response = {
                   playerName: data.playerName || "",
                   playerMetaverseData: [],
                   playerGameData: []
               };
   
               if (data.playerMetaverseData) {
                   response.playerMetaverseData = data.playerMetaverseData.map(item => ({
                       identifier: item.identifier,
                       value: item.value,
                       values: item.values ? [...item.values] : null
                   }));
               }
   
               if (data.playerGameData) {
                   response.playerGameData = data.playerGameData.map(item => ({
                       identifier: item.identifier,
                       value: item.value,
                       values: item.values ? [...item.values] : null
                   }));
               }
   
               const jsonData = JSON.stringify(response);
               console.log('Sending to Unity:', jsonData);
               
               const bufferSize = lengthBytesUTF8(jsonData) + 1;
               const buffer = _malloc(bufferSize);
               stringToUTF8(jsonData, buffer, bufferSize);
               
               wasmTable.get(callback)(buffer);
           })
           .catch(error => {
               console.error('LoadMetaverseData error:', error);
               wasmTable.get(callback)(0);
           });
   },

   Welwise_SaveMetaverseData: function (playerNamePtr, metaverseDataPtr, gameDataPtr, callback) {
      if (typeof wsdk === 'undefined' ||
         !wsdk.metaversePlayer ||
         !wsdk.metaversePlayer.setGameData) {
         console.warn('Welwise metaverse API not available');
         wasmTable.get(callback)();
         return;
      }

      const playerName = UTF8ToString(playerNamePtr);
      const metaverseData = UTF8ToString(metaverseDataPtr);
      const gameData = UTF8ToString(gameDataPtr);

      wsdk.metaversePlayer.setGameData({
            playerName: playerName,
            playerMetaverseData: [{
               identifier: "metaverse_save",
               value: metaverseData
            }],
            playerGameData: [{
               identifier: "game_save",
               value: gameData
            }]
         })
         .then(() => wasmTable.get(callback)())
         .catch(error => {
            console.error('SaveMetaverseData error:', error);
            wasmTable.get(callback)();
         });
   },

   Welwise_IsMetaverseSupported: function () {
      return wsdk.isMetaverseSupported();
   },
   
   Welwise_ShowMidgameAd: function(openCallback, closeCallback, errorCallback) {
       try {
           if (typeof wsdk === 'undefined' || !wsdk.AdvManager) {
               throw new Error('AdvManager not available');
           }
           
           wsdk.AdvManager.showMidgame({
               callbacks: {
                   onOpen: function() {
                       wasmTable.get(openCallback)();
                   },
                   onClose: function() {
                       wasmTable.get(closeCallback)();
                   },
                   onError: function(error) {
                       wasmTable.get(errorCallback)();
                   }
               }
           });
       } catch (e) {
           console.error('Welwise_ShowMidgameAd error:', e);
           wasmTable.get(errorCallback)();
       }
   },
   
   Welwise_ShowRewardedAd: function(idPtr, openCallback, rewardedCallback, closeCallback, errorCallback) {
       try {
           if (typeof wsdk === 'undefined' || !wsdk.AdvManager) {
               throw new Error('AdvManager not available');
           }
           
           const id = UTF8ToString(idPtr);
           
           wsdk.AdvManager.showRewarded({
               callbacks: {
                   onOpen: function() {
                       wasmTable.get(openCallback)();
                   },
                   onRewarded: function() {
                       wasmTable.get(rewardedCallback)();
                   },
                   onClose: function() {
                       wasmTable.get(closeCallback)();
                   },
                   onError: function(error) {
                       wasmTable.get(errorCallback)();
                   }
               }
           });
       } catch (e) {
           console.error('Welwise_ShowRewardedAd error:', e);
           wasmTable.get(errorCallback)();
       }
   },
   
   Welwise_InitServerTime: function (callback) {
       try {
           if (typeof wsdk === 'undefined') {
               console.error('Welwise SDK not initialized');
               wasmTable.get(callback)(BigInt(Date.now()));
               return;
           }
           
           wsdk.serverTime()
               .then(response => {
                   try {
                       // Извлекаем строку времени из объекта
                       const timeStr = response.serverTime;
                       
                       // Преобразуем строку ISO 8601 в timestamp (миллисекунды)
                       const date = new Date(timeStr);
                       const timestamp = date.getTime();
                       
                       // Проверяем валидность преобразования
                       if (isNaN(timestamp)) {
                           throw new Error(`Invalid time format: ${timeStr}`);
                       }
                       
                       wasmTable.get(callback)(BigInt(timestamp));
                   } catch (e) {
                       console.error('Time conversion error:', e);
                       // Возвращаем текущее время как fallback
                       wasmTable.get(callback)(BigInt(Date.now()));
                   }
               })
               .catch(err => {
                   console.error('Server time init error:', err);
                   // Возвращаем текущее время как fallback
                   wasmTable.get(callback)(BigInt(Date.now()));
               });
       } catch (e) {
           console.error('Welwise_InitServerTime error:', e);
           wasmTable.get(callback)(BigInt(Date.now()));
       }
   },
   
   Welwise_GetServerTime: function (callback) {
       try {
           if (typeof wsdk === 'undefined') {
               console.error('Welwise SDK not initialized');
               return;
           }
           
           wsdk.serverTime()
               .then(response => {
                   try {
                       // Извлекаем строку времени из объекта
                       const timeStr = response.serverTime;
                       
                       const date = new Date(timeStr);
                       const timestamp = date.getTime();
                       
                       if (!isNaN(timestamp)) {
                           wasmTable.get(callback)(BigInt(timestamp));
                       }
                   } catch (e) {
                       console.error('Time conversion error:', e);
                   }
               })
               .catch(err => {
                   console.error('Server time update error:', err);
               });
       } catch (e) {
           console.error('Welwise_GetServerTime error:', e);
       }
   },
  
   WelwiseGames_GetLanguage: function () {
      try {
         let lang = "en";
         if (typeof wsdk !== 'undefined' && wsdk.Environment) {
            lang = wsdk.Environment.LanguageCode || lang;
         } else {
            lang = navigator.language || lang;
         }
   
         var buffer = _malloc(lang.length + 1);
         stringToUTF8(lang, buffer, lang.length + 1);
         return buffer;
      } catch (e) {
         console.error('WelwiseGames_GetLanguage error:', e);
         var fallback = "en";
         var buffer = _malloc(fallback.length + 1);
         stringToUTF8(fallback, buffer, fallback.length + 1);
         return buffer;
      }
   },
   
   Welwise_GoToGame: function(gameId, callback) {
       try {
           if (typeof wsdk === 'undefined' || !wsdk.PlatformNavigation || !wsdk.PlatformNavigation.goToGame) {
               throw new Error('PlatformNavigation not available');
           }
   
           wsdk.PlatformNavigation.goToGame(Number(gameId))
               .then(() => {
                   wasmTable.get(callback)(0); // Успех - передаем нулевой указатель
               })
               .catch(error => {
                   let errorMsg = (error && error.message) ? error.message : String(error);
                   var bufferSize = lengthBytesUTF8(errorMsg) + 1;
                   var buffer = _malloc(bufferSize);
                   stringToUTF8(errorMsg, buffer, bufferSize);
                   wasmTable.get(callback)(buffer); // Ошибка - передаем указатель на строку
               });
       } catch (e) {
           let errorMsg = (e && e.message) ? e.message : String(e);
           var bufferSize = lengthBytesUTF8(errorMsg) + 1;
           var buffer = _malloc(bufferSize);
           stringToUTF8(errorMsg, buffer, bufferSize);
           wasmTable.get(callback)(buffer);
       }
   }
});