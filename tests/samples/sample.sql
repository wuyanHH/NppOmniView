-- ==============================================================================
-- NppOmniView SQL 源码高亮与代码编辑测试套件
-- 涵盖 DDL 表定义、复杂 CTE 公用表表达式、窗口分析函数、多表 JOIN 与事务
-- ==============================================================================

-- 1. 数据表结构定义 (DDL)
CREATE TABLE IF NOT EXISTS sys_document_audit_logs (
    log_id BIGINT PRIMARY KEY AUTO_INCREMENT COMMENT '审计主键 ID',
    buffer_id INT NOT NULL COMMENT 'Scintilla 缓冲区句柄',
    file_name VARCHAR(255) NOT NULL COMMENT '文档文件物理名称',
    file_extension VARCHAR(32) NOT NULL COMMENT '文档格式扩展名',
    file_size_bytes INT UNSIGNED DEFAULT 0 COMMENT '文件字节大小',
    format_type ENUM('MARKDOWN', 'JSON', 'CSV', 'SVG', 'YAML', 'MERMAID', 'HTML', 'CODE') NOT NULL COMMENT '识别格式',
    render_latency_ms DECIMAL(8, 2) NOT NULL DEFAULT 0.00 COMMENT '渲染解析耗时',
    is_success TINYINT(1) NOT NULL DEFAULT 1 COMMENT '是否成功渲染',
    error_message TEXT DEFAULT NULL COMMENT '异常堆栈摘要',
    client_ip VARCHAR(45) DEFAULT '127.0.0.1' COMMENT '操作客户端 IP',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '记录生成时间',
    INDEX idx_doc_format (format_type, created_at),
    INDEX idx_buffer_created (buffer_id, created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='文档渲染审计流流水表';

-- 2. 复杂多维度聚合与窗口分析查询 (CTE + Window Functions)
WITH DailyFormatMetrics AS (
    SELECT
        DATE(created_at) AS metric_date,
        format_type,
        COUNT(log_id) AS total_requests,
        SUM(CASE WHEN is_success = 1 THEN 1 ELSE 0 END) AS success_count,
        ROUND(AVG(render_latency_ms), 2) AS avg_latency_ms,
        MAX(render_latency_ms) AS p100_latency_ms
    FROM sys_document_audit_logs
    WHERE created_at >= DATE_SUB(CURRENT_DATE, INTERVAL 30 DAY)
    GROUP BY DATE(created_at), format_type
),
RankedFormatTraffic AS (
    SELECT
        metric_date,
        format_type,
        total_requests,
        avg_latency_ms,
        ROUND(success_count * 100.0 / NULLIF(total_requests, 0), 2) AS success_rate_pct,
        -- 使用窗口函数计算每日各格式流量排名与累计请求量
        DENSE_RANK() OVER (
            PARTITION BY metric_date
            ORDER BY total_requests DESC
        ) AS traffic_rank_in_day,
        SUM(total_requests) OVER (
            PARTITION BY format_type
            ORDER BY metric_date
            ROWS BETWEEN 6 PRECEDING AND CURRENT ROW
        ) AS rolling_7d_total_requests
    FROM DailyFormatMetrics
)
SELECT
    r.metric_date,
    r.traffic_rank_in_day,
    r.format_type,
    r.total_requests,
    r.rolling_7d_total_requests,
    r.success_rate_pct,
    r.avg_latency_ms,
    CASE
        WHEN r.success_rate_pct >= 99.5 THEN 'HEALTHY (极优)'
        WHEN r.success_rate_pct >= 95.0 THEN 'WARNING (轻微告警)'
        ELSE 'CRITICAL (需立即介入)'
    END AS service_health_status
FROM RankedFormatTraffic r
WHERE r.traffic_rank_in_day <= 5
ORDER BY r.metric_date DESC, r.traffic_rank_in_day ASC;

-- 3. 数据维护与事务处理
START TRANSACTION;

INSERT INTO sys_document_audit_logs (
    buffer_id, file_name, file_extension, file_size_bytes, format_type, render_latency_ms, is_success
) VALUES 
(2001, 'sample.ps1', '.ps1', 2048, 'CODE', 14.20, 1),
(2002, 'sample.mmd', '.mmd', 3192, 'MERMAID', 28.50, 1),
(2003, 'sample.csv', '.csv', 4096, 'CSV', 8.10, 1);

UPDATE sys_document_audit_logs
SET render_latency_ms = 12.00
WHERE buffer_id = 2001;

COMMIT;
