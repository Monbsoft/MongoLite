// MongoDB initialization script for Desktop Viewer
// Creates test database with sample collections and documents

// Switch to test database
db = db.getSiblingDB('testdb');

// Create users collection with sample documents
db.users.insertMany([
  {
    _id: ObjectId(),
    name: "Alice Johnson",
    email: "alice@test.com",
    age: 30,
    city: "Paris",
    country: "France",
    registered: new Date("2023-01-15"),
    active: true,
    preferences: {
      theme: "dark",
      notifications: true,
      language: "fr"
    }
  },
  {
    _id: ObjectId(),
    name: "Bob Smith",
    email: "bob@test.com",
    age: 25,
    city: "Lyon",
    country: "France",
    registered: new Date("2023-03-22"),
    active: true,
    preferences: {
      theme: "light",
      notifications: false,
      language: "en"
    }
  },
  {
    _id: ObjectId(),
    name: "Carol Davis",
    email: "carol@test.com",
    age: 35,
    city: "Marseille",
    country: "France",
    registered: new Date("2023-02-10"),
    active: false,
    preferences: {
      theme: "dark",
      notifications: true,
      language: "fr"
    }
  }
]);

// Create products collection with sample documents
db.products.insertMany([
  {
    _id: ObjectId(),
    title: "Laptop Pro",
    price: 999.99,
    category: "electronics",
    stock: 50,
    description: "High-performance laptop for professionals",
    specifications: {
      cpu: "Intel i7",
      ram: "16GB",
      storage: "512GB SSD",
      display: "15.6 inch"
    },
    available: true,
    tags: ["laptop", "professional", "high-performance"]
  },
  {
    _id: ObjectId(),
    title: "Wireless Mouse",
    price: 29.99,
    category: "accessories",
    stock: 200,
    description: "Ergonomic wireless mouse with precision tracking",
    specifications: {
      type: "optical",
      connectivity: "Bluetooth 5.0",
      battery: "AAA",
      dpi: "1600"
    },
    available: true,
    tags: ["mouse", "wireless", "ergonomic"]
  },
  {
    _id: ObjectId(),
    title: "USB-C Hub",
    price: 49.99,
    category: "accessories",
    stock: 75,
    description: "Multi-port USB-C hub for laptops",
    specifications: {
      ports: "4x USB-A, 1x HDMI, 1x SD card",
      power: "100W PD",
      material: "aluminum"
    },
    available: true,
    tags: ["hub", "usb-c", "multi-port"]
  }
]);

// Create orders collection with sample documents
db.orders.insertMany([
  {
    _id: ObjectId(),
    userId: db.users.findOne({name: "Alice Johnson"})._id,
    orderDate: new Date("2023-11-01"),
    status: "completed",
    total: 1029.98,
    items: [
      {
        productId: db.products.findOne({title: "Laptop Pro"})._id,
        quantity: 1,
        price: 999.99
      },
      {
        productId: db.products.findOne({title: "Wireless Mouse"})._id,
        quantity: 1,
        price: 29.99
      }
    ],
    shipping: {
      address: "123 Rue de la Paix, Paris, France",
      method: "express",
      tracking: "FR123456789"
    }
  },
  {
    _id: ObjectId(),
    userId: db.users.findOne({name: "Bob Smith"})._id,
    orderDate: new Date("2023-11-15"),
    status: "processing",
    total: 49.99,
    items: [
      {
        productId: db.products.findOne({title: "USB-C Hub"})._id,
        quantity: 1,
        price: 49.99
      }
    ],
    shipping: {
      address: "456 Avenue Victor Hugo, Lyon, France",
      method: "standard",
      tracking: "FR987654321"
    }
  }
]);

// Print summary
print("MongoDB Desktop Viewer - Database initialized");
print("Collections created:");
print("- users: " + db.users.countDocuments() + " documents");
print("- products: " + db.products.countDocuments() + " documents");
print("- orders: " + db.orders.countDocuments() + " documents");
print("Database: testdb");
print("Connection: mongodb://admin:password@localhost:27017/testdb");