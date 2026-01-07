-- DB2 initialization script for LAWgrid testing
-- Creates sample tables and data

-- Connect to the database
CONNECT TO lawgrid;

-- Create employees table
CREATE TABLE employees (
    employee_id INTEGER NOT NULL PRIMARY KEY,
    first_name VARCHAR(50),
    last_name VARCHAR(50),
    email VARCHAR(100),
    phone_number VARCHAR(20),
    hire_date DATE,
    job_id VARCHAR(20),
    salary DECIMAL(8,2),
    commission_pct DECIMAL(3,2),
    manager_id INTEGER,
    department_id INTEGER
);

-- Create departments table
CREATE TABLE departments (
    department_id INTEGER NOT NULL PRIMARY KEY,
    department_name VARCHAR(50) NOT NULL,
    manager_id INTEGER,
    location_id INTEGER
);

-- Create products table
CREATE TABLE products (
    product_id INTEGER NOT NULL PRIMARY KEY,
    product_name VARCHAR(100) NOT NULL,
    category VARCHAR(50),
    price DECIMAL(10,2),
    stock_quantity INTEGER,
    created_date TIMESTAMP DEFAULT CURRENT TIMESTAMP
);

-- Create indexes for better query performance
CREATE INDEX idx_employees_department ON employees(department_id);
CREATE INDEX idx_employees_manager ON employees(manager_id);
CREATE INDEX idx_products_category ON products(category);

-- Insert sample department data
INSERT INTO departments (department_id, department_name, manager_id, location_id) VALUES
(10, 'Administration', 200, 1700);

INSERT INTO departments (department_id, department_name, manager_id, location_id) VALUES
(20, 'Marketing', 201, 1800);

INSERT INTO departments (department_id, department_name, manager_id, location_id) VALUES
(30, 'Purchasing', 114, 1700);

INSERT INTO departments (department_id, department_name, manager_id, location_id) VALUES
(40, 'Human Resources', 203, 2400);

INSERT INTO departments (department_id, department_name, manager_id, location_id) VALUES
(50, 'Shipping', 121, 1500);

INSERT INTO departments (department_id, department_name, manager_id, location_id) VALUES
(60, 'IT', 103, 1400);

-- Insert sample employee data
INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(100, 'Steven', 'King', 'steven.king@company.com', '515.123.4567', '2003-06-17', 'AD_PRES', 24000.00, NULL, NULL, 10);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(101, 'Neena', 'Kochhar', 'neena.kochhar@company.com', '515.123.4568', '2005-09-21', 'AD_VP', 17000.00, NULL, 100, 10);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(102, 'Lex', 'De Haan', 'lex.dehaan@company.com', '515.123.4569', '2001-01-13', 'AD_VP', 17000.00, NULL, 100, 10);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(103, 'Alexander', 'Hunold', 'alexander.hunold@company.com', '590.423.4567', '2006-01-03', 'IT_PROG', 9000.00, NULL, 102, 60);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(104, 'Bruce', 'Ernst', 'bruce.ernst@company.com', '590.423.4568', '2007-05-21', 'IT_PROG', 6000.00, NULL, 103, 60);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(105, 'David', 'Austin', 'david.austin@company.com', '590.423.4569', '2005-06-25', 'IT_PROG', 4800.00, NULL, 103, 60);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(106, 'Valli', 'Pataballa', 'valli.pataballa@company.com', '590.423.4560', '2006-02-05', 'IT_PROG', 4800.00, NULL, 103, 60);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(107, 'Diana', 'Lorentz', 'diana.lorentz@company.com', '590.423.5567', '2007-02-07', 'IT_PROG', 4200.00, NULL, 103, 60);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(114, 'Den', 'Raphaely', 'den.raphaely@company.com', '515.127.4561', '2002-12-07', 'PU_MAN', 11000.00, NULL, 100, 30);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(121, 'Adam', 'Fripp', 'adam.fripp@company.com', '650.123.2234', '2005-04-10', 'ST_MAN', 8200.00, NULL, 100, 50);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(200, 'Jennifer', 'Whalen', 'jennifer.whalen@company.com', '515.123.4444', '2003-09-17', 'AD_ASST', 4400.00, NULL, 101, 10);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(201, 'Michael', 'Hartstein', 'michael.hartstein@company.com', '515.123.5555', '2004-02-17', 'MK_MAN', 13000.00, NULL, 100, 20);

INSERT INTO employees (employee_id, first_name, last_name, email, phone_number, hire_date, job_id, salary, commission_pct, manager_id, department_id) VALUES
(203, 'Susan', 'Mavris', 'susan.mavris@company.com', '515.123.7777', '2002-06-07', 'HR_REP', 6500.00, NULL, 101, 40);

-- Insert sample product data
INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1001, 'Laptop Pro 15', 'Electronics', 1299.99, 45);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1002, 'Wireless Mouse', 'Electronics', 29.99, 150);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1003, 'USB-C Hub', 'Electronics', 49.99, 89);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1004, 'Mechanical Keyboard', 'Electronics', 149.99, 67);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1005, '27" Monitor', 'Electronics', 399.99, 34);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1006, 'Desk Chair', 'Furniture', 259.99, 23);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1007, 'Standing Desk', 'Furniture', 599.99, 12);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1008, 'Webcam HD', 'Electronics', 79.99, 56);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1009, 'Headphones', 'Electronics', 199.99, 78);

INSERT INTO products (product_id, product_name, category, price, stock_quantity) VALUES
(1010, 'Desk Lamp', 'Furniture', 45.99, 102);

-- Commit the changes
COMMIT;

-- Display success message
VALUES ('DB2 database initialization completed successfully!');

-- Disconnect
CONNECT RESET;
