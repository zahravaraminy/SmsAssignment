
export default {
  basePath: '/<SmsRateLimiter>/',
  entryPoints: {
    '': () => import('./main.server.mjs')
  },
};
