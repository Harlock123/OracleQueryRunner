-- MySQL initialization script for LAWgrid testing
-- Creates sample tables and data

USE lawgrid_test;

-- Create employees table
CREATE TABLE employees (
    employee_id INT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    email VARCHAR(100),
    phone_number VARCHAR(20),
    hire_date DATE,
    job_id VARCHAR(20),
    salary DECIMAL(8,2),
    commission_pct DECIMAL(3,2),
    manager_id INT,
    department_id INT,
    INDEX idx_employees_department (department_id),
    INDEX idx_employees_manager (manager_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create departments table
CREATE TABLE departments (
    department_id INT AUTO_INCREMENT PRIMARY KEY,
    department_name VARCHAR(50) NOT NULL,
    manager_id INT,
    location_id INT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create products table
CREATE TABLE products (
    product_id INT AUTO_INCREMENT PRIMARY KEY,
    product_name VARCHAR(100) NOT NULL,
    category VARCHAR(50),
    price DECIMAL(10,2),
    stock_quantity INT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_products_category (category)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert sample department data
INSERT INTO departments (department_id, department_name, manager_id, location_id) VALUES
(10, 'Administration', 200, 1700),
(20, 'Marketing', 201, 1800),
(30, 'Purchasing', 114, 1700),
(40, 'Human Resources', 203, 2400),
(50, 'Shipping', 121, 1500),
(60, 'IT', 103, 1400);

-- Reset auto increment for departments
ALTER TABLE departments AUTO_INCREMENT = 100;

-- Insert sample employee data
INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(100, 'Steven', 'King', 'steven.king@company.com', '515.123.4567', '2003-06-17', 'AD_PRES', 24000.00, NULL, NULL, 10),
(101, 'Neena', 'Kochhar', 'neena.kochhar@company.com', '515.123.4568', '2005-09-21', 'AD_VP', 17000.00, NULL, 100, 10),
(102, 'Lex', 'De Haan', 'lex.dehaan@company.com', '515.123.4569', '2001-01-13', 'AD_VP', 17000.00, NULL, 100, 10),
(103, 'Alexander', 'Hunold', 'alexander.hunold@company.com', '590.423.4567', '2006-01-03', 'IT_PROG', 9000.00, NULL, 102, 60),
(104, 'Bruce', 'Ernst', 'bruce.ernst@company.com', '590.423.4568', '2007-05-21', 'IT_PROG', 6000.00, NULL, 103, 60),
(105, 'David', 'Austin', 'david.austin@company.com', '590.423.4569', '2005-06-25', 'IT_PROG', 4800.00, NULL, 103, 60),
(106, 'Valli', 'Pataballa', 'valli.pataballa@company.com', '590.423.4560', '2006-02-05', 'IT_PROG', 4800.00, NULL, 103, 60),
(107, 'Diana', 'Lorentz', 'diana.lorentz@company.com', '590.423.5567', '2007-02-07', 'IT_PROG', 4200.00, NULL, 103, 60),
(114, 'Den', 'Raphaely', 'den.raphaely@company.com', '515.127.4561', '2002-12-07', 'PU_MAN', 11000.00, NULL, 100, 30),
(121, 'Adam', 'Fripp', 'adam.fripp@company.com', '650.123.2234', '2005-04-10', 'ST_MAN', 8200.00, NULL, 100, 50),
(200, 'Jennifer', 'Whalen', 'jennifer.whalen@company.com', '515.123.4444', '2003-09-17', 'AD_ASST', 4400.00, NULL, 101, 10),
(201, 'Michael', 'Hartstein', 'michael.hartstein@company.com', '515.123.5555', '2004-02-17', 'MK_MAN', 13000.00, NULL, 100, 20),
(203, 'Susan', 'Mavris', 'susan.mavris@company.com', '515.123.7777', '2002-06-07', 'HR_REP', 6500.00, NULL, 101, 40);

-- Reset auto increment for employees
ALTER TABLE employees AUTO_INCREMENT = 300;

-- Insert sample product data
INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1001, 'Laptop Pro 15', 'Electronics', 1299.99, 45),
(1002, 'Wireless Mouse', 'Electronics', 29.99, 150),
(1003, 'USB-C Hub', 'Electronics', 49.99, 89),
(1004, 'Mechanical Keyboard', 'Electronics', 149.99, 67),
(1005, '27" Monitor', 'Electronics', 399.99, 34),
(1006, 'Desk Chair', 'Furniture', 259.99, 23),
(1007, 'Standing Desk', 'Furniture', 599.99, 12),
(1008, 'Webcam HD', 'Electronics', 79.99, 56),
(1009, 'Headphones', 'Electronics', 199.99, 78),
(1010, 'Desk Lamp', 'Furniture', 45.99, 102);

-- Reset auto increment for products
ALTER TABLE products AUTO_INCREMENT = 2000;

-- Display success message
SELECT 'MySQL database initialization completed successfully!' AS status;
