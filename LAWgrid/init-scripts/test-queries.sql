-- Test Queries for LAWgrid Oracle Populator Testing
-- You can use these queries to test your LAWgrid Oracle population methods

-- ============================================
-- BASIC QUERIES
-- ============================================

-- Query 1: All Employees
-- Expected: 13 rows
SELECT * FROM EMPLOYEES ORDER BY EMPLOYEE_ID;

-- Query 2: All Departments
-- Expected: 6 rows
SELECT * FROM DEPARTMENTS ORDER BY DEPARTMENT_ID;

-- Query 3: All Products
-- Expected: 10 rows
SELECT * FROM PRODUCTS ORDER BY PRODUCT_ID;

-- ============================================
-- JOIN QUERIES
-- ============================================

-- Query 4: Employees with Department Names
-- Expected: 13 rows with department information
SELECT
    e.EMPLOYEE_ID,
    e.FIRST_NAME || ' ' || e.LAST_NAME AS FULL_NAME,
    e.EMAIL,
    e.HIRE_DATE,
    e.SALARY,
    d.DEPARTMENT_NAME
FROM EMPLOYEES e
LEFT JOIN DEPARTMENTS d ON e.DEPARTMENT_ID = d.DEPARTMENT_ID
ORDER BY e.EMPLOYEE_ID;

-- Query 5: Employees with Manager Names
-- Expected: Shows employee hierarchy
SELECT
    e.EMPLOYEE_ID,
    e.FIRST_NAME || ' ' || e.LAST_NAME AS EMPLOYEE_NAME,
    e.JOB_ID,
    e.SALARY,
    m.FIRST_NAME || ' ' || m.LAST_NAME AS MANAGER_NAME
FROM EMPLOYEES e
LEFT JOIN EMPLOYEES m ON e.MANAGER_ID = m.EMPLOYEE_ID
ORDER BY e.EMPLOYEE_ID;

-- ============================================
-- FILTERED QUERIES
-- ============================================

-- Query 6: IT Department Only
-- Expected: 5 rows
SELECT
    FIRST_NAME,
    LAST_NAME,
    EMAIL,
    SALARY,
    HIRE_DATE
FROM EMPLOYEES
WHERE DEPARTMENT_ID = 60
ORDER BY SALARY DESC;

-- Query 7: High Earners (Salary > 10000)
-- Expected: 5 rows
SELECT
    FIRST_NAME || ' ' || LAST_NAME AS EMPLOYEE_NAME,
    JOB_ID,
    SALARY,
    DEPARTMENT_ID
FROM EMPLOYEES
WHERE SALARY > 10000
ORDER BY SALARY DESC;

-- Query 8: Electronics Products
-- Expected: 8 rows
SELECT
    PRODUCT_NAME,
    PRICE,
    STOCK_QUANTITY,
    PRICE * STOCK_QUANTITY AS TOTAL_VALUE
FROM PRODUCTS
WHERE CATEGORY = 'Electronics'
ORDER BY TOTAL_VALUE DESC;

-- ============================================
-- AGGREGATE QUERIES
-- ============================================

-- Query 9: Employees Count by Department
-- Expected: 6 rows showing counts
SELECT
    d.DEPARTMENT_NAME,
    COUNT(e.EMPLOYEE_ID) AS EMPLOYEE_COUNT,
    AVG(e.SALARY) AS AVERAGE_SALARY,
    MAX(e.SALARY) AS MAX_SALARY,
    MIN(e.SALARY) AS MIN_SALARY
FROM DEPARTMENTS d
LEFT JOIN EMPLOYEES e ON d.DEPARTMENT_ID = e.DEPARTMENT_ID
GROUP BY d.DEPARTMENT_NAME
ORDER BY EMPLOYEE_COUNT DESC;

-- Query 10: Products Summary by Category
-- Expected: 2 rows (Electronics, Furniture)
SELECT
    CATEGORY,
    COUNT(*) AS PRODUCT_COUNT,
    SUM(STOCK_QUANTITY) AS TOTAL_STOCK,
    AVG(PRICE) AS AVG_PRICE,
    MIN(PRICE) AS MIN_PRICE,
    MAX(PRICE) AS MAX_PRICE
FROM PRODUCTS
GROUP BY CATEGORY
ORDER BY CATEGORY;

-- ============================================
-- DATE-BASED QUERIES
-- ============================================

-- Query 11: Recently Hired Employees (after 2005)
-- Expected: 5 rows
SELECT
    FIRST_NAME || ' ' || LAST_NAME AS EMPLOYEE_NAME,
    HIRE_DATE,
    MONTHS_BETWEEN(SYSDATE, HIRE_DATE) AS MONTHS_EMPLOYED,
    SALARY
FROM EMPLOYEES
WHERE HIRE_DATE >= TO_DATE('2005-01-01', 'YYYY-MM-DD')
ORDER BY HIRE_DATE DESC;

-- Query 12: Employees by Hire Year
-- Expected: Groups by year
SELECT
    EXTRACT(YEAR FROM HIRE_DATE) AS HIRE_YEAR,
    COUNT(*) AS EMPLOYEES_HIRED,
    AVG(SALARY) AS AVERAGE_SALARY
FROM EMPLOYEES
GROUP BY EXTRACT(YEAR FROM HIRE_DATE)
ORDER BY HIRE_YEAR;

-- ============================================
-- COMPLEX QUERIES
-- ============================================

-- Query 13: Department Statistics with Product Value
-- Expected: Complex result showing department and product stats
SELECT
    'DEPARTMENT_STATS' AS DATA_TYPE,
    d.DEPARTMENT_NAME AS NAME,
    COUNT(e.EMPLOYEE_ID) AS COUNT,
    AVG(e.SALARY) AS AVERAGE_VALUE
FROM DEPARTMENTS d
LEFT JOIN EMPLOYEES e ON d.DEPARTMENT_ID = e.DEPARTMENT_ID
GROUP BY d.DEPARTMENT_NAME
UNION ALL
SELECT
    'PRODUCT_CATEGORY' AS DATA_TYPE,
    CATEGORY AS NAME,
    COUNT(*) AS COUNT,
    AVG(PRICE) AS AVERAGE_VALUE
FROM PRODUCTS
GROUP BY CATEGORY
ORDER BY DATA_TYPE, NAME;

-- Query 14: Top 5 Highest Paid Employees
-- Expected: 5 rows
SELECT *
FROM (
    SELECT
        FIRST_NAME || ' ' || LAST_NAME AS EMPLOYEE_NAME,
        JOB_ID,
        SALARY,
        DEPARTMENT_ID,
        RANK() OVER (ORDER BY SALARY DESC) AS SALARY_RANK
    FROM EMPLOYEES
)
WHERE SALARY_RANK <= 5;

-- ============================================
-- TESTING DIFFERENT DATA TYPES
-- ============================================

-- Query 15: All Data Types Test
-- Expected: 1 row showing various Oracle data types
SELECT
    SYSDATE AS CURRENT_DATE,
    TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:MI:SS') AS FORMATTED_DATE,
    1234567890 AS LARGE_NUMBER,
    123.456 AS DECIMAL_NUMBER,
    'Test String' AS VARCHAR_DATA,
    NULL AS NULL_VALUE,
    CASE WHEN 1=1 THEN 'TRUE' ELSE 'FALSE' END AS BOOLEAN_LIKE
FROM DUAL;
