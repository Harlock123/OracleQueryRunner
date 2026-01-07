// Test Queries for LAWgrid MongoDB Populator Testing
// Use these in mongosh or as filter parameters in LAWgrid

// Switch to the test database
use lawgrid_test;

// ============================================
// BASIC QUERIES (use {} as filter in LAWgrid)
// ============================================

// Query 1: All Employees (demonstrates varied schemas)
// Filter: {}
db.employees.find({});

// Query 2: All Departments
// Filter: {}
db.departments.find({});

// Query 3: All Products (highly varied schemas)
// Filter: {}
db.products.find({});

// Query 4: All Orders
// Filter: {}
db.orders.find({});

// ============================================
// FILTERED QUERIES
// ============================================

// Query 5: IT Department Employees Only
// Filter: {"department_id": 60}
db.employees.find({ department_id: 60 });

// Query 6: High Earners (Salary > 10000)
// Filter: {"salary": {"$gt": 10000}}
db.employees.find({ salary: { $gt: 10000 } });

// Query 7: Electronics Products
// Filter: {"category": "Electronics"}
db.products.find({ category: "Electronics" });

// Query 8: Furniture Products
// Filter: {"category": "Furniture"}
db.products.find({ category: "Furniture" });

// Query 9: Employees Hired After 2005
// Filter: {"hire_date": {"$gte": {"$date": "2005-01-01T00:00:00Z"}}}
db.employees.find({ hire_date: { $gte: new Date('2005-01-01') } });

// Query 10: Products in Stock (quantity > 50)
// Filter: {"stock_quantity": {"$gt": 50}}
db.products.find({ stock_quantity: { $gt: 50 } });

// ============================================
// QUERIES DEMONSTRATING VARIED SCHEMAS
// ============================================

// Query 11: Employees with Skills (only some have this field)
// Filter: {"skills": {"$exists": true}}
// This will show only employees with the skills field
db.employees.find({ skills: { $exists: true } });

// Query 12: Employees with Commission (only some have this field)
// Filter: {"commission_pct": {"$exists": true}}
db.employees.find({ commission_pct: { $exists: true } });

// Query 13: Products with Warranty (only some have this field)
// Filter: {"warranty_years": {"$exists": true}}
db.products.find({ warranty_years: { $exists: true } });

// Query 14: Products with Specifications (nested document)
// Filter: {"specifications": {"$exists": true}}
db.products.find({ specifications: { $exists: true } });

// Query 15: Contract Employees (have contract_end_date)
// Filter: {"employment_type": "Contract"}
db.employees.find({ employment_type: "Contract" });

// ============================================
// COMPLEX QUERIES
// ============================================

// Query 16: Products by Multiple Categories
// Filter: {"category": {"$in": ["Electronics", "Furniture"]}}
db.products.find({ category: { $in: ["Electronics", "Furniture"] } });

// Query 17: Employees with Manager
// Filter: {"manager_id": {"$exists": true}}
db.employees.find({ manager_id: { $exists: true } });

// Query 18: High-Value Orders (> $200)
// Filter: {"total_amount": {"$gt": 200}}
db.orders.find({ total_amount: { $gt: 200 } });

// Query 19: Orders with Tracking Number
// Filter: {"tracking_number": {"$exists": true}}
db.orders.find({ tracking_number: { $exists: true } });

// Query 20: Products with Color Options
// Filter: {"color_options": {"$exists": true}}
db.products.find({ color_options: { $exists: true } });

// ============================================
// PROJECTION QUERIES (not directly supported in LAWgrid, but useful for mongosh)
// ============================================

// Query 21: Employee Names and Salaries Only
db.employees.find({}, { first_name: 1, last_name: 1, salary: 1, _id: 0 });

// Query 22: Product Names and Prices Only
db.products.find({}, { product_name: 1, price: 1, _id: 0 });

// ============================================
// AGGREGATE QUERIES (for reference - LAWgrid uses find())
// ============================================

// Query 23: Count Employees by Department
db.employees.aggregate([
  { $group: { _id: "$department_id", count: { $sum: 1 } } },
  { $sort: { count: -1 } }
]);

// Query 24: Average Salary by Department
db.employees.aggregate([
  { $group: { _id: "$department_id", avg_salary: { $avg: "$salary" } } },
  { $sort: { _id: 1 } }
]);

// Query 25: Product Count by Category
db.products.aggregate([
  { $group: { _id: "$category", count: { $sum: 1 } } },
  { $sort: { _id: 1 } }
]);

// ============================================
// ARRAY FIELD QUERIES
// ============================================

// Query 26: Products with Multiple Color Options
// Filter: {"color_options": {"$exists": true, "$ne": []}}
db.products.find({ color_options: { $exists: true, $ne: [] } });

// Query 27: Employees with Specific Skill (if skills field exists)
// Filter: {"skills": "Python"}
db.products.find({ skills: "Python" });

// ============================================
// NESTED DOCUMENT QUERIES
// ============================================

// Query 28: Products with Specific Specification
// Filter: {"specifications.processor": {"$exists": true}}
db.products.find({ "specifications.processor": { $exists: true } });

// Query 29: Orders with Shipping Address
// Filter: {"shipping_address": {"$exists": true}}
db.orders.find({ shipping_address: { $exists: true } });

// ============================================
// REGEX QUERIES
// ============================================

// Query 30: Employees with Email Containing "company"
// Filter: {"email": {"$regex": "company", "$options": "i"}}
db.employees.find({ email: { $regex: "company", $options: "i" } });

// Query 31: Products Starting with "Laptop"
// Filter: {"product_name": {"$regex": "^Laptop", "$options": "i"}}
db.products.find({ product_name: { $regex: "^Laptop", $options: "i" } });

// ============================================
// USEFUL MONGOSH COMMANDS
// ============================================

// Show all collections
show collections;

// Count documents in a collection
db.employees.countDocuments();
db.products.countDocuments();

// Get all unique field names in employees collection (shows schema variety)
db.employees.aggregate([
  { $project: { fields: { $objectToArray: "$$ROOT" } } },
  { $unwind: "$fields" },
  { $group: { _id: "$fields.k" } },
  { $sort: { _id: 1 } }
]);

// Find documents with the most fields
db.employees.aggregate([
  { $project: {
      doc: "$$ROOT",
      fieldCount: { $size: { $objectToArray: "$$ROOT" } }
    }
  },
  { $sort: { fieldCount: -1 } },
  { $limit: 5 }
]);

print("\n===== MongoDB Test Queries Loaded =====");
print("Use these filters in LAWgrid's PopulateFromMongoQuery methods");
print("Example: grid.PopulateFromMongoQuery(connectionString, 'lawgrid_test', 'employees', '{}');");
print("Example: grid.PopulateFromMongoQuery(connectionString, 'lawgrid_test', 'employees', '{\"department_id\": 60}');");
