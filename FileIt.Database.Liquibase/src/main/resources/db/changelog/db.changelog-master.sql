-- Liquibase Master Changelog for FileIt Database
-- This file orchestrates all database migrations

--liquibase formatted sql

--changeset fileit:1-create-hhf-tables
-- Create initial HHF (Holder Holdings Flow) tables

--changeset fileit:1-create-hhf-holders runOnChange:false
CREATE TABLE IF NOT EXISTS hhf_holders
(
    holder_id      VARCHAR(30) NOT NULL PRIMARY KEY,
    customer_name  VARCHAR(200) NOT NULL, 
    account_number VARCHAR(50) NOT NULL, 
    address        VARCHAR(200) NULL, 
    city           VARCHAR(100) NULL, 
    state          VARCHAR(50) NULL, 
    zip            VARCHAR(20) NULL, 
    created_at     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    account_closed TIMESTAMP NULL,
    updated_at     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
--rollback DROP TABLE IF EXISTS hhf_holders CASCADE;

--changeset fileit:1-create-hhf-holdings runOnChange:false
CREATE TABLE IF NOT EXISTS hhf_holdings
(
    id             SERIAL PRIMARY KEY,
    holder_id      VARCHAR(30)       NOT NULL REFERENCES hhf_holders(holder_id),
    cusip_or_symbol VARCHAR(50)       NOT NULL, 
    name           VARCHAR(200)      NULL, 
    quantity       NUMERIC(38, 32)   NOT NULL, 
    as_of_date     TIMESTAMP         NOT NULL DEFAULT CURRENT_DATE::TIMESTAMP
);
--rollback DROP TABLE IF EXISTS hhf_holdings CASCADE;

--changeset fileit:1-create-hhf-presentvalue runOnChange:false
CREATE TABLE IF NOT EXISTS hhf_presentvalue
(
    id               SERIAL PRIMARY KEY,
    cusip_or_symbol  VARCHAR(50)       NOT NULL, 
    unit_price       NUMERIC(12, 2)    NOT NULL, 
    dividend_multiple NUMERIC(38, 32)   NOT NULL, 
    split_multiple    NUMERIC(38, 32)   NOT NULL, 
    cumulative_splits NUMERIC(38, 32)   NOT NULL, 
    risk_scalar       NUMERIC(38, 32)   NULL, 
    as_of_date        TIMESTAMP         NOT NULL DEFAULT CURRENT_DATE::TIMESTAMP
);
--rollback DROP TABLE IF EXISTS hhf_presentvalue CASCADE;

 INSERT INTO hhf_presentvalue ( cusip_or_symbol, unit_price, dividend_multiple, split_multiple, cumulative_splits, as_of_date ) VALUES ( 'NVDA', 950.2500, 1.000000000000, 1.00000000, 1.0, '2026-05-09' );