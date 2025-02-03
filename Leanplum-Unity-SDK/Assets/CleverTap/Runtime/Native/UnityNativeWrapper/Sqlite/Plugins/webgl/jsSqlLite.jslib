mergeInto(LibraryManager.library, {
    Init: function(dbName) { init(Pointer_stringify(dbName)); },
    SetCurrentTable: function(tableName) { setCurrentTable(Pointer_stringify(tableName)); },
    Insert: function(entry) { return allocate(intArrayFromString(insertEntry(Pointer_stringify(entry)).toString()), ALLOC_NORMAL); },
    Delete: function(id) { return allocate(intArrayFromString(deleteEntry(id).toString()), ALLOC_NORMAL); },
    GetAllEntries: function() { return allocate(intArrayFromString(getAllEntries()), ALLOC_NORMAL); }
});