const baseUrl = "/api/Manager/";

export const managerService = {
  getClients: function() {
    return $.post(`${baseUrl}GetClients`);
  },
  kickClient: function(userId) {
    return $.post(`${baseUrl}KickClient/${userId}`);
  }
};