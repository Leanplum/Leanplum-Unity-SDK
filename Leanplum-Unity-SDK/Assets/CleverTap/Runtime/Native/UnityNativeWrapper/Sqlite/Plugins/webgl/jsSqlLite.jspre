let db;
let collections = {};
let currentTable = null;


// Function to initialize the database
function init(dbName) {
    const savedDb = localStorage.getItem(dbName);
    if (savedDb) {
        db = new loki(dbName);
        db.loadJSON(savedDb);
        console.log(`Database ${dbName} loaded from localStorage.`);
        // Populate collections from the loaded database
        db.collections.forEach(collection => {
            collections[collection.name] = collection;
        });
    } else {
        db = new loki(dbName);
        console.log(`Database ${dbName} initialized.`);
    }
}

// Save the database state to localStorage
function saveDb() {
    const serializedDb = db.serialize();
    localStorage.setItem(db.filename, serializedDb);
    console.log(`Database ${db.filename} saved to localStorage.`);
}

// Function to set the current table
function setCurrentTable(tableName) {
    if (!db) {
        console.error("Database is not initialized.");
        return;
    }
    currentTable = tableName;
    if (!collections[tableName]) {
        collections[tableName] = db.addCollection(tableName, {
            autoupdate: true,
            unique: ['Id']
        });
        console.log(`Table ${tableName} created.`);
    } else {
        console.log(`Table ${tableName} already exists.`);
    }
    saveDb();
}

// Function to insert data with auto-increment ID
function insertEntry(jsonEntry) {
    if (!currentTable) {
        console.error("Current table is not set.");
        return -1;
    }

    console.log('Inserting entry:', jsonEntry);

    const entry = JSON.parse(jsonEntry);
    let collection = collections[currentTable];

    entry.Id = generateNextId();
    collection.insert(entry);
    console.log(`Entry inserted into ${currentTable}:`, entry);
    saveDb();
    return entry.Id;
}

// Function to delete data by ID
function deleteEntry(Id) {
    if (!currentTable) {
        console.error("Current table is not set.");
        return -1;
    }
    let collection = collections[currentTable];
    const item = collection.findOne({ Id: Id });
    if (item) {
        collection.remove(item);
        console.log(`Entry with ID ${Id} deleted from ${currentTable}.`);
        saveDb();
        return Id;
    } else {
        console.error(`Entry with ID ${Id} not found in ${currentTable}.`);
        return -1;
    }
}

// Function to get all entries from the current table
function getAllEntries() {
    if (!currentTable) {
        console.error("Current table is not set.");
        return JSON.stringify({});
    }
    let collection = collections[currentTable];
    // Create a dictionary of entries with Id as the key
    let entries = {};
    collection.find().forEach(entry => {
        entries[entry.Id] = entry;
    });
    return JSON.stringify(entries);
}

function generateNextId() {
    let lastIdKey = "lastRecordId";
    let id = parseInt(localStorage.getItem(lastIdKey), 10) || 0;
    id += 1;
    localStorage.setItem(lastIdKey, id);
    return id;
}

// Helper function to allocate memory for a string and return a pointer
function allocateString(str) {
    var size = lengthBytesUTF8(str) + 1;
    var ptr = _malloc(size);
    stringToUTF8(str, ptr, size);
    return ptr;
}