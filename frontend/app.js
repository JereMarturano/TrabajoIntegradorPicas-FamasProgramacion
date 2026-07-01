// =========================================================
// PICAS & FAMAS — Frontend
// Vanilla JS, sin dependencias. Habla con la API ASP.NET Core
// definida en src/PicasYFamas.Api (Controllers/AuthController.cs,
// Controllers/GamesController.cs).
// =========================================================

const DEFAULT_API_BASE = 'http://localhost:5163/api/game/v1';

const LS = {
  apiBase: 'pyf_apiBase',
  token: 'pyf_token',
  playerId: 'pyf_playerId',
  gameId: 'pyf_gameId',
  crt: 'pyf_crt',
  bg: 'pyf_bg',
  sound: 'pyf_sound',
};

const state = {
  apiBase: localStorage.getItem(LS.apiBase) || DEFAULT_API_BASE,
  token: localStorage.getItem(LS.token) || null,
  playerId: localStorage.getItem(LS.playerId) || null,
  gameId: localStorage.getItem(LS.gameId) || null,
  crt: localStorage.getItem('pyf_crt') !== 'false',
  bg: localStorage.getItem('pyf_bg') !== 'false',
  sound: localStorage.getItem('pyf_sound') === 'true',
};

// ---------------------------------------------------------
// Helpers de DOM
// ---------------------------------------------------------
const $ = (sel) => document.querySelector(sel);
const $$ = (sel) => Array.from(document.querySelectorAll(sel));

function show(el) { el.classList.remove('hidden'); }
function hide(el) { el.classList.add('hidden'); }

function toast(msg) {
  const t = $('#toast');
  t.textContent = msg;
  show(t);
  clearTimeout(toast._timer);
  toast._timer = setTimeout(() => hide(t), 3600);
}

// ---------------------------------------------------------
// Cliente API
// ---------------------------------------------------------
async function api(path, { method = 'GET', body, auth = false } = {}) {
  const headers = { 'Content-Type': 'application/json' };
  if (auth) {
    if (!state.token) throw new Error('No hay sesión activa.');
    headers['Authorization'] = `Bearer ${state.token}`;
  }

  let res;
  try {
    res = await fetch(`${state.apiBase}${path}`, {
      method,
      headers,
      body: body ? JSON.stringify(body) : undefined,
    });
  } catch (networkErr) {
    throw new Error('No se pudo conectar con el servidor. Revisá la URL de la API (⚙) y que el backend esté corriendo.');
  }

  let payload = null;
  try { payload = await res.json(); } catch (_) { /* sin cuerpo */ }

  if (!res.ok || (payload && payload.success === false)) {
    const msg = payload?.errors?.[0]?.message || `Error ${res.status}`;
    if (res.status === 401) logout(true);
    throw new Error(msg);
  }

  return payload?.data;
}

// ---------------------------------------------------------
// Configuración de servidor (modal)
// ---------------------------------------------------------
$('#apiUrl').value = state.apiBase;
$('#cfgCrt').checked = state.crt;
$('#cfgBg').checked = state.bg;
$('#cfgSound').checked = state.sound;

function applyVisualSettings() {
  const scanlines = $('.scanlines');
  if (scanlines) scanlines.style.display = state.crt ? 'block' : 'none';
  const stage = $('.stage');
  if (stage) stage.style.animation = state.crt ? 'crtFlicker 6s infinite' : 'none';
  const bgfx = $('#bgfx');
  if (bgfx) bgfx.style.display = state.bg ? 'block' : 'none';
}
applyVisualSettings();

$('#cfgBtn').addEventListener('click', () => show($('#cfgModal')));
$$('[data-close]').forEach(btn => btn.addEventListener('click', () => {
  hide(document.getElementById(btn.dataset.close));
}));
$('#saveCfgBtn').addEventListener('click', () => {
  const val = $('#apiUrl').value.trim().replace(/\/+$/, '');
  if (!val) { toast('Ingresá una URL válida.'); return; }
  
  state.apiBase = val;
  localStorage.setItem(LS.apiBase, val);
  
  state.crt = $('#cfgCrt').checked;
  localStorage.setItem(LS.crt, state.crt);
  
  state.bg = $('#cfgBg').checked;
  localStorage.setItem(LS.bg, state.bg);
  
  state.sound = $('#cfgSound').checked;
  localStorage.setItem(LS.sound, state.sound);
  
  applyVisualSettings();

  hide($('#cfgModal'));
  toast('Configuración guardada.');
});

// ---------------------------------------------------------
// Tabs login / registro
// ---------------------------------------------------------
$('#tabLogin').addEventListener('click', () => switchTab('login'));
$('#tabRegister').addEventListener('click', () => switchTab('register'));

function switchTab(tab) {
  const isLogin = tab === 'login';
  $('#tabLogin').classList.toggle('tab--active', isLogin);
  $('#tabRegister').classList.toggle('tab--active', !isLogin);
  $('#loginForm').classList.toggle('hidden', !isLogin);
  $('#registerForm').classList.toggle('hidden', isLogin);
}

// ---------------------------------------------------------
// Login
// ---------------------------------------------------------
$('#loginForm').addEventListener('submit', async (e) => {
  e.preventDefault();
  const errEl = $('#loginError');
  errEl.textContent = '';

  const email = $('#loginEmail').value.trim();
  const password = $('#loginPassword').value;

  if (!email || !password) {
    errEl.textContent = 'Completá email y contraseña.';
    return;
  }

  setLoading('#loginSubmit', true);
  try {
    const data = await api('/login', { method: 'POST', body: { email, password } });
    onAuthSuccess(data);
  } catch (err) {
    errEl.textContent = err.message;
  } finally {
    setLoading('#loginSubmit', false);
  }
});

// ---------------------------------------------------------
// Registro
// ---------------------------------------------------------
$('#registerForm').addEventListener('submit', async (e) => {
  e.preventDefault();
  const errEl = $('#registerError');
  errEl.textContent = '';

  const firstname = $('#regFirstname').value.trim();
  const lastname = $('#regLastname').value.trim();
  const age = parseInt($('#regAge').value, 10);
  const email = $('#regEmail').value.trim();
  const password = $('#regPassword').value;

  if (!firstname || !lastname || !email || !password || !age) {
    errEl.textContent = 'Completá todos los campos.';
    return;
  }

  setLoading('#registerSubmit', true);
  try {
    const data = await api('/register', {
      method: 'POST',
      body: { firstname, lastname, age, email, password },
    });
    onAuthSuccess(data);
  } catch (err) {
    errEl.textContent = err.message;
  } finally {
    setLoading('#registerSubmit', false);
  }
});

function setLoading(selector, loading) {
  const btn = $(selector);
  const label = btn.querySelector('.btn__label');
  if (!btn.dataset.label) btn.dataset.label = label.textContent;
  btn.disabled = loading;
  label.textContent = loading ? '...' : btn.dataset.label;
}

function onAuthSuccess(data) {
  state.token = data.token;
  state.playerId = data.playerId;
  localStorage.setItem(LS.token, data.token);
  localStorage.setItem(LS.playerId, data.playerId);
  enterGameView();
}

function logout(silent) {
  state.token = null;
  state.playerId = null;
  state.gameId = null;
  localStorage.removeItem(LS.token);
  localStorage.removeItem(LS.playerId);
  localStorage.removeItem(LS.gameId);
  hide($('#view-game'));
  show($('#view-auth'));
  $('#loginForm').reset();
  $('#registerForm').reset();
  if (!silent) toast('Sesión cerrada.');
}

$('#logoutBtn').addEventListener('click', () => logout(false));

// ---------------------------------------------------------
// Vista de juego
// ---------------------------------------------------------
function enterGameView() {
  hide($('#view-auth'));
  show($('#view-game'));
  $('#playerIdShort').textContent = state.playerId ? state.playerId.slice(0, 8) + '…' : '—';

  if (state.gameId) {
    restoreGame();
  } else {
    showNoGamePanel();
  }
}

function showNoGamePanel() {
  show($('#noGamePanel'));
  hide($('#gamePanel'));
  $('#gameStatusLabel').textContent = 'SIN PARTIDA';
  $('#attemptsPill').textContent = '0 intentos';
}

function showGamePanel() {
  hide($('#noGamePanel'));
  show($('#gamePanel'));
}

$('#startGameBtn').addEventListener('click', async () => {
  setLoading('#startGameBtn', true);
  try {
    const data = await api('/start', { method: 'POST', auth: true });
    state.gameId = data.gameId;
    localStorage.setItem(LS.gameId, data.gameId);
    $('#scoreboard').innerHTML = '';
    $('#gameStatusLabel').textContent = 'EN CURSO';
    $('#attemptsPill').textContent = '0 intentos';
    showGamePanel();
    clearDigits();
    focusFirstDigit();
  } catch (err) {
    toast(err.message);
  } finally {
    setLoading('#startGameBtn', false);
  }
});

async function restoreGame() {
  try {
    const game = await api(`/${state.gameId}`, { auth: true });
    const guesses = await api(`/${state.gameId}/guesses`, { auth: true });

    $('#gameStatusLabel').textContent = translateStatus(game.status);
    $('#attemptsPill').textContent = `${guesses.length} intento${guesses.length === 1 ? '' : 's'}`;
    renderScoreboard(guesses);
    showGamePanel();
    clearDigits();

    const finished = guesses.some(g => g.isWinner) || /won|ganad/i.test(game.status || '');
    if (finished) {
      $('#guessForm').classList.add('hidden');
    } else {
      $('#guessForm').classList.remove('hidden');
      focusFirstDigit();
    }
  } catch (err) {
    // La partida guardada ya no existe o no es válida
    state.gameId = null;
    localStorage.removeItem(LS.gameId);
    showNoGamePanel();
  }
}

function translateStatus(status) {
  if (!status) return 'EN CURSO';
  const s = status.toLowerCase();
  if (s.includes('won') || s.includes('finish') || s.includes('ganad')) return 'GANADA';
  if (s.includes('progress') || s.includes('curso')) return 'EN CURSO';
  return status.toUpperCase();
}

// ---------------------------------------------------------
// Input de dígitos (4 casilleros, auto-avance)
// ---------------------------------------------------------
const digitInputs = $$('.digit');

digitInputs.forEach((input, idx) => {
  input.addEventListener('input', () => {
    input.value = input.value.replace(/[^0-9]/g, '').slice(0, 1);
    input.classList.toggle('digit--filled', input.value !== '');
    input.classList.remove('digit--error');
    if (input.value && idx < digitInputs.length - 1) {
      digitInputs[idx + 1].focus();
    }
  });

  input.addEventListener('keydown', (e) => {
    if (e.key === 'Backspace' && !input.value && idx > 0) {
      digitInputs[idx - 1].focus();
    }
  });

  input.addEventListener('paste', (e) => {
    const text = (e.clipboardData.getData('text') || '').replace(/[^0-9]/g, '');
    if (text.length >= 1) {
      e.preventDefault();
      text.slice(0, 4).split('').forEach((ch, i) => {
        if (digitInputs[i]) {
          digitInputs[i].value = ch;
          digitInputs[i].classList.add('digit--filled');
        }
      });
      const next = digitInputs[Math.min(text.length, 3)];
      next.focus();
    }
  });
});

function clearDigits() {
  digitInputs.forEach(d => { d.value = ''; d.classList.remove('digit--filled', 'digit--error'); });
}
function focusFirstDigit() { digitInputs[0]?.focus(); }

function shakeDigits() {
  digitInputs.forEach(d => d.classList.add('digit--error'));
  setTimeout(() => digitInputs.forEach(d => d.classList.remove('digit--error')), 260);
}

// ---------------------------------------------------------
// Envío de intento
// ---------------------------------------------------------
$('#guessForm').addEventListener('submit', async (e) => {
  e.preventDefault();
  const errEl = $('#guessError');
  errEl.textContent = '';

  const guess = digitInputs.map(d => d.value).join('');

  if (guess.length !== 4) {
    errEl.textContent = 'Completá los 4 dígitos.';
    shakeDigits();
    return;
  }
  if (new Set(guess).size !== 4) {
    errEl.textContent = 'Los 4 dígitos deben ser distintos.';
    shakeDigits();
    return;
  }

  setLoading('#guessSubmit', true);
  try {
    const result = await api('/guess', {
      method: 'POST',
      auth: true,
      body: { gameId: state.gameId, attemptedNumber: guess },
    });

    const guesses = await api(`/${state.gameId}/guesses`, { auth: true });
    renderScoreboard(guesses);
    $('#attemptsPill').textContent = `${guesses.length} intento${guesses.length === 1 ? '' : 's'}`;
    clearDigits();

    if (result.isFinished) {
      $('#gameStatusLabel').textContent = 'GANADA';
      $('#guessForm').classList.add('hidden');
      $('#winSub').textContent = `${result.message} · ${guesses.length} intento${guesses.length === 1 ? '' : 's'}`;
      show($('#winOverlay'));
    } else {
      focusFirstDigit();
    }
  } catch (err) {
    errEl.textContent = err.message;
    shakeDigits();
  } finally {
    setLoading('#guessSubmit', false);
  }
});

$('#winPlayAgainBtn').addEventListener('click', async () => {
  hide($('#winOverlay'));
  $('#guessForm').classList.remove('hidden');
  try {
    const data = await api('/start', { method: 'POST', auth: true });
    state.gameId = data.gameId;
    localStorage.setItem(LS.gameId, data.gameId);
    $('#scoreboard').innerHTML = '';
    $('#gameStatusLabel').textContent = 'EN CURSO';
    $('#attemptsPill').textContent = '0 intentos';
    showGamePanel();
    clearDigits();
    focusFirstDigit();
  } catch (err) {
    toast(err.message);
  }
});

// ---------------------------------------------------------
// Marcador de intentos (scoreboard)
// ---------------------------------------------------------
function renderScoreboard(guesses) {
  const board = $('#scoreboard');
  board.innerHTML = '';
  guesses
    .slice()
    .sort((a, b) => a.attemptNumber - b.attemptNumber)
    .forEach(g => board.appendChild(buildScoreRow(g)));
}

function buildScoreRow(g) {
  const row = document.createElement('div');
  row.className = 'scoreRow' + (g.isWinner ? ' scoreRow--winner' : '');

  const num = document.createElement('span');
  num.className = 'scoreRow__num';
  num.textContent = `#${g.attemptNumber}`;
  row.appendChild(num);

  const digitsWrap = document.createElement('span');
  digitsWrap.className = 'scoreRow__digits';
  String(g.guess).split('').forEach(ch => {
    const d = document.createElement('span');
    d.className = 'scoreRow__digit';
    d.textContent = ch;
    digitsWrap.appendChild(d);
  });
  row.appendChild(digitsWrap);

  const badges = document.createElement('span');
  badges.className = 'scoreRow__badges';
  badges.innerHTML = `
    <span class="badge badge--picas">${g.picas}P</span>
    <span class="badge badge--famas">${g.famas}F</span>
  `;
  row.appendChild(badges);

  return row;
}

// ---------------------------------------------------------
// Fondo: estrellas pixeladas flotando
// ---------------------------------------------------------
(function bgfx() {
  const canvas = $('#bgfx');
  const ctx = canvas.getContext('2d');
  let stars = [];
  const COLORS = ['#ff3e88', '#36e2e2', '#ffd23f', '#fdf0dc'];

  function resize() {
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    const count = Math.floor((canvas.width * canvas.height) / 18000);
    stars = Array.from({ length: count }, () => ({
      x: Math.random() * canvas.width,
      y: Math.random() * canvas.height,
      size: Math.random() < 0.85 ? 2 : 3,
      color: COLORS[Math.floor(Math.random() * COLORS.length)],
      speed: 0.05 + Math.random() * 0.15,
      phase: Math.random() * Math.PI * 2,
    }));
  }

  function tick(t) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    stars.forEach(s => {
      const alpha = 0.25 + 0.5 * Math.abs(Math.sin(t * 0.0005 * s.speed * 10 + s.phase));
      ctx.globalAlpha = alpha;
      ctx.fillStyle = s.color;
      ctx.fillRect(Math.floor(s.x), Math.floor(s.y), s.size, s.size);
    });
    ctx.globalAlpha = 1;
    requestAnimationFrame(tick);
  }

  window.addEventListener('resize', resize);
  resize();
  requestAnimationFrame(tick);
})();

// ---------------------------------------------------------
// Arranque
// ---------------------------------------------------------
(function init() {
  if (state.token && state.playerId) {
    enterGameView();
  }
})();
