mergeInto(LibraryManager.library, {
	SaveDataExtern: function (date){

		var dateString = UTF8ToString(date);
		var myObj = JSON.parse(dateString);

		console.log("dateString - " + dateString);

		player.setData(myObj);
	},

	LoadDataExtern: function(objName){
		objName = UTF8ToString(objName);

		if(player != null){
			player.getData().then(_data => {
				const MyJSON = JSON.stringify(_data);
				console.log("JSON - " + MyJSON);
				MyGameInstance.SendMessage(objName, "Load", MyJSON);
			});
		}
		else{
			MyGameInstance.SendMessage(objName, "LoadLocal");
		}

		
		
	},
});