mergeInto(LibraryManager.library, {
    Init: function(dbName) { init(UTF8ToString(dbName)); },
    SetCurrentTable: function(tableName) { setCurrentTable(UTF8ToString(tableName)); },
    Insert: function(entry) { 
        return allocateString(insertEntry(UTF8ToString(entry)).toString());
    },
    Delete: function(id) { 
        return allocateString(deleteEntry(id).toString());
    },
    GetAllEntries: function() { 
        return allocateString(getAllEntries());
    }
});