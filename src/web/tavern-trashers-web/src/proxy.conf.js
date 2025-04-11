module.exports = {
  "/api": {
    target:
      process.env["services__gateway__https__0"] ||
      process.env["services__gateway__http__0"],
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api": "",
    },
  },
};
