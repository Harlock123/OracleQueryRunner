// MongoDB initialization script for LAWgrid testing
// Demonstrates documents with different schemas in the same collection

// Switch to the lawgrid_test database
db = db.getSiblingDB('lawgrid_test');

// Create a non-admin user for testing
db.createUser({
  user: 'testuser',
  pwd: 'TestPassword123',
  roles: [
    {
      role: 'readWrite',
      db: 'lawgrid_test'
    }
  ]
});

// ============================================
// EMPLOYEES COLLECTION
// Demonstrates varied schemas - some employees have different fields
// ============================================

db.employees.insertMany([
  // Standard employee record - has all common fields
  {
    employee_id: 100,
    first_name: 'Steven',
    last_name: 'King',
    email: 'steven.king@company.com',
    phone_number: '515.123.4567',
    hire_date: new Date('2003-06-17'),
    job_id: 'AD_PRES',
    salary: 24000,
    department_id: 10
  },

  // Employee with commission field (not in all records)
  {
    employee_id: 101,
    first_name: 'Neena',
    last_name: 'Kochhar',
    email: 'neena.kochhar@company.com',
    phone_number: '515.123.4568',
    hire_date: new Date('2005-09-21'),
    job_id: 'AD_VP',
    salary: 17000,
    commission_pct: 0.15,
    manager_id: 100,
    department_id: 10
  },

  // Employee with additional certification field
  {
    employee_id: 102,
    first_name: 'Lex',
    last_name: 'De Haan',
    email: 'lex.dehaan@company.com',
    hire_date: new Date('2001-01-13'),
    job_id: 'AD_VP',
    salary: 17000,
    manager_id: 100,
    department_id: 10,
    certifications: ['PMP', 'MBA']
  },

  // IT employee with tech skills array (unique to tech staff)
  {
    employee_id: 103,
    first_name: 'Alexander',
    last_name: 'Hunold',
    email: 'alexander.hunold@company.com',
    phone_number: '590.423.4567',
    hire_date: new Date('2006-01-03'),
    job_id: 'IT_PROG',
    salary: 9000,
    manager_id: 102,
    department_id: 60,
    skills: ['C#', 'Python', 'SQL'],
    remote_work_eligible: true
  },

  // Employee with performance rating (newer field not in all records)
  {
    employee_id: 104,
    first_name: 'Bruce',
    last_name: 'Ernst',
    email: 'bruce.ernst@company.com',
    hire_date: new Date('2007-05-21'),
    job_id: 'IT_PROG',
    salary: 6000,
    manager_id: 103,
    department_id: 60,
    performance_rating: 4.5,
    last_review_date: new Date('2024-10-01')
  },

  // Minimal employee record - only required fields
  {
    employee_id: 105,
    first_name: 'David',
    last_name: 'Austin',
    email: 'david.austin@company.com',
    hire_date: new Date('2005-06-25'),
    salary: 4800,
    department_id: 60
  },

  // Employee with emergency contact (new field)
  {
    employee_id: 106,
    first_name: 'Valli',
    last_name: 'Pataballa',
    email: 'valli.pataballa@company.com',
    phone_number: '590.423.4560',
    hire_date: new Date('2006-02-05'),
    job_id: 'IT_PROG',
    salary: 4800,
    manager_id: 103,
    department_id: 60,
    emergency_contact: {
      name: 'Jane Pataballa',
      phone: '555-0123',
      relationship: 'Spouse'
    }
  },

  // Employee with previous employment history
  {
    employee_id: 107,
    first_name: 'Diana',
    last_name: 'Lorentz',
    email: 'diana.lorentz@company.com',
    hire_date: new Date('2007-02-07'),
    job_id: 'IT_PROG',
    salary: 4200,
    department_id: 60,
    previous_employers: ['TechCorp', 'DataSystems']
  },

  // Contract employee (has contract_end_date instead of permanent fields)
  {
    employee_id: 108,
    first_name: 'John',
    last_name: 'Contractor',
    email: 'john.contractor@company.com',
    hire_date: new Date('2024-01-15'),
    contract_end_date: new Date('2025-01-15'),
    hourly_rate: 75,
    department_id: 60,
    employment_type: 'Contract'
  },

  // Part-time employee
  {
    employee_id: 109,
    first_name: 'Sarah',
    last_name: 'PartTime',
    email: 'sarah.parttime@company.com',
    phone_number: '555-9999',
    hire_date: new Date('2024-03-01'),
    salary: 2400,
    department_id: 40,
    hours_per_week: 20,
    employment_type: 'Part-Time'
  }
]);

// ============================================
// DEPARTMENTS COLLECTION
// Standard schema for all documents
// ============================================

db.departments.insertMany([
  {
    department_id: 10,
    department_name: 'Administration',
    manager_id: 100,
    location_id: 1700
  },
  {
    department_id: 20,
    department_name: 'Marketing',
    manager_id: 201,
    location_id: 1800
  },
  {
    department_id: 30,
    department_name: 'Purchasing',
    manager_id: 114,
    location_id: 1700
  },
  {
    department_id: 40,
    department_name: 'Human Resources',
    manager_id: 203,
    location_id: 2400
  },
  {
    department_id: 50,
    department_name: 'Shipping',
    manager_id: 121,
    location_id: 1500
  },
  {
    department_id: 60,
    department_name: 'IT',
    manager_id: 103,
    location_id: 1400,
    budget: 500000,
    floor: 3
  }
]);

// ============================================
// PRODUCTS COLLECTION
// Highly varied schemas - different product types have different attributes
// ============================================

db.products.insertMany([
  // Electronics product with tech specs
  {
    product_id: 1001,
    product_name: 'Laptop Pro 15',
    category: 'Electronics',
    price: 1299.99,
    stock_quantity: 45,
    specifications: {
      processor: 'Intel i7',
      ram: '16GB',
      storage: '512GB SSD',
      screen_size: '15.6 inch'
    },
    warranty_years: 3
  },

  // Electronics with different specs
  {
    product_id: 1002,
    product_name: 'Wireless Mouse',
    category: 'Electronics',
    price: 29.99,
    stock_quantity: 150,
    specifications: {
      dpi: 1600,
      wireless: true,
      battery_life: '12 months'
    },
    color_options: ['Black', 'White', 'Silver']
  },

  // Minimal electronics product
  {
    product_id: 1003,
    product_name: 'USB-C Hub',
    category: 'Electronics',
    price: 49.99,
    stock_quantity: 89
  },

  // Electronics with energy rating
  {
    product_id: 1004,
    product_name: 'Mechanical Keyboard',
    category: 'Electronics',
    price: 149.99,
    stock_quantity: 67,
    switch_type: 'Cherry MX Blue',
    backlit: true,
    energy_star_rating: 'A+'
  },

  // Monitor with display specs
  {
    product_id: 1005,
    product_name: '27" Monitor',
    category: 'Electronics',
    price: 399.99,
    stock_quantity: 34,
    specifications: {
      resolution: '2560x1440',
      refresh_rate: '144Hz',
      panel_type: 'IPS',
      hdr: true
    },
    warranty_years: 2,
    vesa_mount_compatible: true
  },

  // Furniture product - completely different schema
  {
    product_id: 1006,
    product_name: 'Desk Chair',
    category: 'Furniture',
    price: 259.99,
    stock_quantity: 23,
    material: 'Mesh and Steel',
    weight_capacity: 300,
    adjustable_height: true,
    color: 'Black',
    assembly_required: true
  },

  // Furniture with dimensions
  {
    product_id: 1007,
    product_name: 'Standing Desk',
    category: 'Furniture',
    price: 599.99,
    stock_quantity: 12,
    dimensions: {
      width: 60,
      depth: 30,
      min_height: 28,
      max_height: 48
    },
    material: 'Bamboo and Steel',
    electric_adjustment: true,
    weight_capacity: 200
  },

  // Camera - unique fields for photography
  {
    product_id: 1008,
    product_name: 'Webcam HD',
    category: 'Electronics',
    price: 79.99,
    stock_quantity: 56,
    resolution: '1080p',
    frame_rate: 60,
    auto_focus: true,
    built_in_microphone: true
  },

  // Audio product
  {
    product_id: 1009,
    product_name: 'Headphones',
    category: 'Electronics',
    price: 199.99,
    stock_quantity: 78,
    driver_size: '40mm',
    frequency_response: '20Hz-20kHz',
    noise_cancelling: true,
    bluetooth_version: '5.0',
    battery_life_hours: 30
  },

  // Furniture minimal
  {
    product_id: 1010,
    product_name: 'Desk Lamp',
    category: 'Furniture',
    price: 45.99,
    stock_quantity: 102,
    color_temperature: '3000K-6000K',
    dimmable: true
  },

  // Book product - completely different schema
  {
    product_id: 2001,
    product_name: 'MongoDB Guide',
    category: 'Books',
    price: 39.99,
    stock_quantity: 200,
    author: 'John Smith',
    publisher: 'Tech Books Inc',
    isbn: '978-1234567890',
    pages: 450,
    publication_year: 2024,
    format: 'Paperback'
  },

  // Software product - subscription based
  {
    product_id: 3001,
    product_name: 'Office Suite Pro',
    category: 'Software',
    price: 99.99,
    is_subscription: true,
    billing_period: 'Annual',
    platforms: ['Windows', 'Mac', 'Linux'],
    max_devices: 5,
    cloud_storage_gb: 1000
  }
]);

// ============================================
// ORDERS COLLECTION
// Demonstrates embedded documents and arrays with varied structures
// ============================================

db.orders.insertMany([
  // Order with multiple items
  {
    order_id: 1,
    customer_name: 'Alice Johnson',
    order_date: new Date('2024-11-01'),
    items: [
      { product_id: 1001, quantity: 1, price: 1299.99 },
      { product_id: 1002, quantity: 2, price: 29.99 }
    ],
    total_amount: 1359.97,
    status: 'Delivered',
    shipping_address: {
      street: '123 Main St',
      city: 'Springfield',
      state: 'IL',
      zip: '62701'
    }
  },

  // Order with discount applied
  {
    order_id: 2,
    customer_name: 'Bob Williams',
    order_date: new Date('2024-11-05'),
    items: [
      { product_id: 1007, quantity: 1, price: 599.99 }
    ],
    subtotal: 599.99,
    discount_code: 'SAVE10',
    discount_amount: 60.00,
    total_amount: 539.99,
    status: 'Shipped',
    tracking_number: 'TRACK123456'
  },

  // Simple order - minimal fields
  {
    order_id: 3,
    customer_name: 'Carol Davis',
    order_date: new Date('2024-11-10'),
    items: [
      { product_id: 1003, quantity: 3, price: 49.99 }
    ],
    total_amount: 149.97,
    status: 'Processing'
  },

  // Order with gift wrapping
  {
    order_id: 4,
    customer_name: 'David Miller',
    order_date: new Date('2024-11-12'),
    items: [
      { product_id: 1009, quantity: 1, price: 199.99 }
    ],
    total_amount: 209.98,
    status: 'Pending',
    gift_wrap: true,
    gift_message: 'Happy Birthday!',
    gift_wrap_fee: 9.99
  }
]);

// Create indexes for better query performance
db.employees.createIndex({ employee_id: 1 }, { unique: true });
db.employees.createIndex({ department_id: 1 });
db.employees.createIndex({ email: 1 }, { unique: true });

db.departments.createIndex({ department_id: 1 }, { unique: true });

db.products.createIndex({ product_id: 1 }, { unique: true });
db.products.createIndex({ category: 1 });

db.orders.createIndex({ order_id: 1 }, { unique: true });
db.orders.createIndex({ customer_name: 1 });

print('MongoDB lawgrid_test database initialized successfully!');
print('Collections created: employees, departments, products, orders');
print('User created: testuser');
print('');
print('Connection string: mongodb://testuser:TestPassword123@localhost:27017/lawgrid_test');
