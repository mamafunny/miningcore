\c miningcore
ALTER TABLE blocks ADD COLUMN IF NOT EXISTS minereffort FLOAT NULL;
