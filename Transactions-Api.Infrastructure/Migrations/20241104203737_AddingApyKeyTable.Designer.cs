﻿// <auto-generated />
using System;
using Transactions_Api.Infrastructure.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Transactions_API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241104203737_AddingApyKeyTable")]
    partial class AddingApyKeyTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Transactions_API.Models.ApiKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cnpj")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("varchar(14)")
                        .HasColumnName("cnpj");

                    b.Property<string>("Conta")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("conta");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)")
                        .HasColumnName("apikey");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("nome");

                    b.HasKey("Id");

                    b.ToTable("apikeys");
                });

            modelBuilder.Entity("Transactions_API.Models.Transacao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DataTransacao")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("data_transacao");

                    b.Property<string>("E2eId")
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)")
                        .HasColumnName("e2e_id");

                    b.Property<string>("PagadorAgencia")
                        .HasMaxLength(6)
                        .HasColumnType("varchar(6)")
                        .HasColumnName("pagador_agencia");

                    b.Property<string>("PagadorBanco")
                        .HasMaxLength(8)
                        .HasColumnType("varchar(8)")
                        .HasColumnName("pagador_banco");

                    b.Property<string>("PagadorConta")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("pagador_conta");

                    b.Property<string>("PagadorCpf")
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11)")
                        .HasColumnName("pagador_cpf");

                    b.Property<string>("PagadorNome")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("pagador_nome");

                    b.Property<string>("RecebedorAgencia")
                        .HasMaxLength(6)
                        .HasColumnType("varchar(6)")
                        .HasColumnName("recebedor_agencia");

                    b.Property<string>("RecebedorBanco")
                        .HasMaxLength(8)
                        .HasColumnType("varchar(8)")
                        .HasColumnName("recebedor_banco");

                    b.Property<string>("RecebedorConta")
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("recebedor_conta");

                    b.Property<string>("RecebedorCpf")
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11)")
                        .HasColumnName("recebedor_cpf");

                    b.Property<string>("RecebedorNome")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("recebedor_nome");

                    b.Property<string>("Txid")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("varchar(35)")
                        .HasColumnName("txid");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(10,2)")
                        .HasColumnName("valor");

                    b.HasKey("Id");

                    b.HasIndex("Txid")
                        .IsUnique();

                    b.ToTable("transacoes");
                });
#pragma warning restore 612, 618
        }
    }
}
