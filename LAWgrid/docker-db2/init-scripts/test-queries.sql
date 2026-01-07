-- Test Queries for LAWgrid DB2 Populator Testing

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
    e.first_name || ' ' || e.last_name AS full_name,
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
    e.first_name || ' ' || e.last_name AS employee_name,
    e.job_id,
    e.salary,
    m.first_name || ' ' || m.last_name AS manager_name
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
    first_name || ' ' || last_name AS employee_name,
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
    DECIMAL(AVG(e.salary), 8, 2) AS average_salary,
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
    DECIMAL(AVG(price), 10, 2) AS avg_price,
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
    first_name || ' ' || last_name AS employee_name,
    hire_date,
    (YEAR(CURRENT DATE) - YEAR(hire_date)) * 12 +
    (MONTH(CURRENT DATE) - MONTH(hire_date)) AS months_employed,
    salary
FROM employees
WHERE hire_date >= '2005-01-01'
ORDER BY hire_date DESC;

-- Query 12: Employees by Hire Year
SELECT
    YEAR(hire_date) AS hire_year,
    COUNT(*) AS employees_hired,
    DECIMAL(AVG(salary), 8, 2) AS average_salary
FROM employees
GROUP BY YEAR(hire_date)
ORDER BY hire_year;

-- ============================================
-- DB2-SPECIFIC FEATURES
-- ============================================

-- Query 13: Using LISTAGG (DB2 11.1+)
SELECT
    d.department_name,
    LISTAGG(e.first_name || ' ' || e.last_name, ', ')
        WITHIN GROUP (ORDER BY e.employee_id) AS employees
FROM departments d
LEFT JOIN employees e ON d.department_id = e.department_id
GROUP BY d.department_name
ORDER BY d.department_name;

-- Query 14: Window Functions
SELECT
    first_name || ' ' || last_name AS employee_name,
    job_id,
    salary,
    department_id,
    RANK() OVER (ORDER BY salary DESC) AS salary_rank,
    RANK() OVER (PARTITION BY department_id ORDER BY salary DESC) AS dept_salary_rank
FROM employees
ORDER BY salary DESC
FETCH FIRST 10 ROWS ONLY;

-- Query 15: Various Data Types
SELECT
    CURRENT TIMESTAMP AS current_time,
    VARCHAR_FORMAT(CURRENT TIMESTAMP, 'YYYY-MM-DD HH24:MI:SS') AS formatted_time,
    1234567890 AS large_number,
    DECIMAL(123.456, 6, 3) AS decimal_number,
    'Test String' AS varchar_data,
    CAST(NULL AS VARCHAR(10)) AS null_value
FROM SYSIBM.SYSDUMMY1;

-- Query 16: FETCH FIRST syntax (DB2-specific pagination)
SELECT
    product_name,
    category,
    price,
    stock_quantity
FROM products
ORDER BY price DESC
FETCH FIRST 5 ROWS ONLY;

-- Query 17: WITH clause (Common Table Expression)
WITH high_earners AS (
    SELECT
        employee_id,
        first_name || ' ' || last_name AS employee_name,
        salary,
        department_id
    FROM employees
    WHERE salary > 8000
)
SELECT
    he.employee_name,
    he.salary,
    d.department_name
FROM high_earners he
JOIN departments d ON he.department_id = d.department_id
ORDER BY he.salary DESC;

-- Query 18: CASE expression
SELECT
    first_name || ' ' || last_name AS employee_name,
    salary,
    CASE
        WHEN salary >= 15000 THEN 'Executive'
        WHEN salary >= 10000 THEN 'Senior'
        WHEN salary >= 5000 THEN 'Mid-Level'
        ELSE 'Junior'
    END AS salary_grade
FROM employees
ORDER BY salary DESC;
