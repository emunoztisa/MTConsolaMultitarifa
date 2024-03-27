CREATE TABLE ct_andadores(
	[pkAndador] [int] IDENTITY(1,1) NOT NULL,
	[fkLugar] [int] NULL,
	[fkStatus] [int] NULL,
	[nombre] [nvarchar](100) NOT NULL,
	[deleted_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[created_at] [nvarchar](50) NULL
	PRIMARY KEY (pkAndador)
)
CREATE TABLE ct_corredores(
	[pkCorredor] [int] IDENTITY(1,1) NOT NULL,
	[fkEmpresa] [int] NOT NULL,
	[nombre] [nvarchar](20) NOT NULL,
	[status] [int] NULL,
	[deleted_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[created_at] [nvarchar](50) NULL
	PRIMARY KEY (pkCorredor)
)
CREATE TABLE ct_empresas(
	[pkEmpresa] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](20) NOT NULL,
	[status] [int] NULL,
	[deleted_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[created_at] [nvarchar](50) NULL
	PRIMARY KEY (pkEmpresa)
)
CREATE TABLE ct_lugares(
	[pkLugar] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](200) NOT NULL,
	[status] [int] NULL,
	[deleted_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[created_at] [nvarchar](50) NULL
	PRIMARY KEY (pkLugar)
)
CREATE TABLE ct_operadores(
	[pkOperador] [int] IDENTITY(1,1) NOT NULL,
	[fkEmpresa] [int] NULL,
	[nombre] [nvarchar](100) NOT NULL,
	[status] [int] NULL,
	[created_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[deleted_at] [nvarchar](50) NULL
	PRIMARY KEY (pkOperador)
)
CREATE TABLE ct_perfiles(
	[pkPerfil] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](200) NOT NULL,
	[status] [int] NULL,
	[deleted_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[created_at] [nvarchar](50) NULL
	PRIMARY KEY (pkPerfil)
)
CREATE TABLE ct_Rutas(
	[pkRuta] [int] IDENTITY(1,1) NOT NULL,
	[fkCorredor] [int] NULL,
	[nombre] [nvarchar](200) NOT NULL,
	[status] [int] NULL,
	[created_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[deleted_at] [nvarchar](50) NULL
	PRIMARY KEY (pkRuta)
)
CREATE TABLE ct_unidades(
	[pkUnidad] [int] IDENTITY(1,1) NOT NULL,
	[fkEmpresa] [int] NULL,
	[fkCorredor] [int] NULL,
	[nombre] [nvarchar](20) NULL,
	[noSerieAVL] [nvarchar](10) NULL,
	[economico] [nvarchar](10) NULL,
	[capacidad] [tinyint] NULL,
	[validador] [int] NULL,
	[status] [int] NULL,
	[created_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[deleted_at] [nvarchar](50) NULL
	PRIMARY KEY (pkUnidad)
)
CREATE TABLE sy_asignaciones(
	[pkAsignacion] [bigint] IDENTITY(1,1) NOT NULL,
	[fkRuta] [int] NOT NULL,
	[fkUnidad] [int] NOT NULL,
	[fkOperador] [int] NOT NULL,
	[fkAndador] [int] NULL,
	[fkLiquidacion] [bigint] NULL,
	[fkStatus] [int] NULL,
	[folio] [nvarchar](50) NULL,
	[fecha] [nvarchar](50) NULL,
	[hora] [nvarchar](50) NULL,
	[cantidadAsientosDisp] [int] NULL,
	[recurrente] [int] NULL,
	[created_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[deleted_at] [nvarchar](50) NULL,
	PRIMARY KEY (pkAsignacion)
)
CREATE TABLE sy_tarifas(
	[pkTarifa] [bigint] IDENTITY(1,1) NOT NULL,
	[fkLugarOrigen] [int] NOT NULL,
	[fkLugarDestino] [int] NOT NULL,
	[fkPerfil] [int] NOT NULL,
	[monto] [decimal](18, 2) NOT NULL,
	[status] [int] NULL,
	[created_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[deleted_at] [nvarchar](50) NULL,
	PRIMARY KEY (pkTarifa)
)
CREATE TABLE sy_lugar_ruta(
	[pkLugarRuta] [bigint] IDENTITY(1,1) NOT NULL,
	[fkLugar] [int] NOT NULL,
	[fkRuta] [int] NOT NULL,
	[orden] [int] NOT NULL,
	[status] [int] NULL,
	[deleted_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[created_at] [nvarchar](50) NULL,
	PRIMARY KEY (pkLugarRuta)
)
CREATE TABLE sy_empresa_corredor_operador(
	[pkEmpresaCorredorOperador] [bigint] IDENTITY(1,1) NOT NULL,
	[fkEmpresa] [int] NOT NULL,
	[fkCorredor] [int] NOT NULL,
	[fkOperador] [int] NOT NULL,
	[status] [int] NULL,
	[created_at] [nvarchar](50) NULL,
	[updated_at] [nvarchar](50) NULL,
	[deleted_at] [nvarchar](50) NULL,
	PRIMARY KEY (pkEmpresaCorredorOperador)
)