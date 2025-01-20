INSERT INTO transactionsapi.apikeys (id, apikey, nome, cnpj, conta)
SELECT 1, '123456', 'Mauricio', '1231456789', '45687978'
WHERE NOT EXISTS (
    SELECT 1
    FROM transactionsapi.apikeys
    WHERE id = 1
);

commit;