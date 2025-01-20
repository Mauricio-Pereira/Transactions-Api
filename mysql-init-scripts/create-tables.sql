create table if not exists __efmigrationshistory
(
    MigrationId    varchar(150) not null
        primary key,
    ProductVersion varchar(32)  not null
);

create table if not exists apikeys
(
    id     int auto_increment
        primary key,
    apikey varchar(64)  not null,
    nome   varchar(100) not null,
    cnpj   varchar(14)  not null,
    conta  varchar(10)  not null
);

create table if not exists transacoes
(
    id                  int auto_increment
        primary key,
    txid                varchar(35)    not null,
    e2e_id              varchar(64)    null,
    pagador_nome        varchar(100)   null,
    pagador_documento   varchar(11)    null,
    pagador_banco       varchar(8)     null,
    pagador_agencia     varchar(6)     null,
    pagador_conta       varchar(10)    null,
    recebedor_nome      varchar(100)   null,
    recebedor_documento varchar(11)    null,
    recebedor_banco     varchar(8)     null,
    recebedor_agencia   varchar(6)     null,
    recebedor_conta     varchar(10)    null,
    valor               decimal(10, 2) not null,
    data_transacao      timestamp      not null,
    constraint IX_transacoes_txid
        unique (txid)
);

commit;