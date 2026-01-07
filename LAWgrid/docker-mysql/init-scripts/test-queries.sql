-- Test Queries for LAWgrid MySQL Populator Testing

-- ============================================
-- BASIC QUERIES
-- ============================================

-- Query 1: All Employees
SELECT * FROM employees ORDER BY employee_id;

-- Query 2: All Departments
SELECT * FROM departments ORDER BY department_id;

-- Query 3: All Products
SELECT * FROM products ORDER BY product_id;

-- ============================================
-- JOIN QUERIES
-- ============================================

-- Query 4: Employees with Department Names
SELECT
    e.employee_id,
    CONCAT(e.first_name, ' ', e.last_name) AS full_name,
    e.email,
    e.hire_date,
    e.salary,
    d.department_name
FROM employees e
LEFT JOIN departments d ON e.department_id = d.department_id
ORDER BY e.employee_id;

-- Query 5: Employees with Manager Names
SELECT
    e.employee_id,
    CONCAT(e.first_name, ' ', e.last_name) AS employee_name,
    e.job_id,
    e.salary,
    CONCAT(m.first_name, ' ', m.last_name) AS manager_name
FROM employees e
LEFT JOIN employees m ON e.manager_id = m.employee_id
ORDER BY e.employee_id;

-- ============================================
-- FILTERED QUERIES
-- ============================================

-- Query 6: IT Department Only
SELECT
    first_name,
    last_name,
    email,
    salary,
    hire_date
FROM employees
WHERE department_id = 60
ORDER BY salary DESC;

-- Query 7: High Earners (Salary > 10000)
SELECT
    CONCAT(first_name, ' ', last_name) AS employee_name,
    job_id,
    salary,
    department_id
FROM employees
WHERE salary > 10000
ORDER BY salary DESC;

-- Query 8: Electronics Products
SELECT
    product_name,
    price,
    stock_quantity,
    price * stock_quantity AS total_value
FROM products
WHERE category = 'Electronics'
ORDER BY total_value DESC;

-- ============================================
-- AGGREGATE QUERIES
-- ============================================

-- Query 9: Employees Count by Department
SELECT
    d.department_name,
    COUNT(e.employee_id) AS employee_count,
    ROUND(AVG(e.salary), 2) AS average_salary,
    MAX(e.salary) AS max_salary,
    MIN(e.salary) AS min_salary
FROM departments d
LEFT JOIN employees e ON d.department_id = e.department_id
GROUP BY d.department_name
ORDER BY employee_count DESC;

-- Query 10: Products Summary by Category
SELECT
    category,
    COUNT(*) AS product_count,
    SUM(stock_quantity) AS total_stock,
    ROUND(AVG(price), 2) AS avg_price,
    MIN(price) AS min_price,
    MAX(price) AS max_price
FROM products
GROUP BY category
ORDER BY category;

-- ============================================
-- DATE-BASED QUERIES
-- ============================================

-- Query 11: Recently Hired Employees (after 2005)
SELECT
    CONCAT(first_name, ' ', last_name) AS employee_name,
    hire_date,
    TIMESTAMPDIFF(MONTH, hire_date, CURDATE()) AS months_employed,
    salary
FROM employees
WHERE hire_date >= '2005-01-01'
ORDER BY hire_date DESC;

-- Query 12: Employees by Hire Year
SELECT
    YEAR(hire_date) AS hire_year,
    COUNT(*) AS employees_hired,
    ROUND(AVG(salary), 2) AS average_salary
FROM employees
GROUP BY YEAR(hire_date)
ORDER BY hire_year;

-- ============================================
-- MYSQL-SPECIFIC FEATURES
-- ============================================

-- Query 13: Using JSON functions (MySQL 5.7+)
SELECT
    d.department_name,
    JSON_ARRAYAGG(
        JSON_OBJECT(
            'name', CONCAT(e.first_name, ' ', e.last_name),
            'salary', e.salary
        )
    ) AS employees
FROM departments d
LEFT JOIN employees e ON d.department_id = e.department_id
GROUP BY d.department_name
ORDER BY d.department_name;

-- Query 14: Window Functions (MySQL 8.0+)
SELECT
    CONCAT(first_name, ' ', last_name) AS employee_name,
    job_id,
    salary,
    department_id,
    RANK() OVER (ORDER BY salary DESC) AS salary_rank,
    RANK() OVER (PARTITION BY department_id ORDER BY salary DESC) AS dept_salary_rank
FROM employees
ORDER BY salary DESC
LIMIT 10;

-- Query 15: Various Data Types
SELECT
    NOW() AS current_time,
    DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i:%s') AS formatted_time,
    1234567890 AS large_number,
    123.456 AS decimal_number,
    'Test String' AS varchar_data,
    NULL AS null_value,
    CAST('{"key": "value"}' AS JSON) AS json_data;

-- Query 16: Full-Text Search (if needed)
-- Note: Requires FULLTEXT index
-- ALTER TABLE products ADD FULLTEXT(product_name);
-- SELECT product_name, category, price
-- FROM products
-- WHERE MATCH(product_name) AGAINST('laptop');
