import './styles.css';

declare const __SERVICE_BASE__: string;

let ws: WebSocket | null = null;

const wsUrlInput = document.getElementById('wsUrl') as HTMLInputElement;
const connectBtn = document.getElementById('connectBtn') as HTMLButtonElement;
const disconnectBtn = document.getElementById('disconnectBtn') as HTMLButtonElement;
const statusDiv = document.getElementById('status') as HTMLDivElement;
const messageInput = document.getElementById('messageInput') as HTMLInputElement;
const sendBtn = document.getElementById('sendBtn') as HTMLButtonElement;
const messagesDiv = document.getElementById('messages') as HTMLDivElement;

// Initialize URL input with default WebSocket URL
if (__SERVICE_BASE__) {
    const wsUrl = __SERVICE_BASE__.replace(/^http/, 'ws') + '/ws';
    wsUrlInput.value = wsUrl;
}

function updateStatus(status: 'connected' | 'disconnected' | 'connecting') {
    statusDiv.className = `status ${status}`;
    statusDiv.textContent = status.charAt(0).toUpperCase() + status.slice(1);
}

function addMessage(type: 'sent' | 'received' | 'error', content: string) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${type}`;
    
    const label = document.createElement('div');
    label.className = 'message-label';
    label.textContent = type === 'sent' ? 'Sent' : type === 'received' ? 'Received' : 'Error';
    
    const contentDiv = document.createElement('div');
    contentDiv.className = 'message-content';
    contentDiv.textContent = content;
    
    messageDiv.appendChild(label);
    messageDiv.appendChild(contentDiv);
    messagesDiv.appendChild(messageDiv);
    messagesDiv.scrollTop = messagesDiv.scrollHeight;
}

function connect() {
    const url = wsUrlInput.value.trim();
    if (!url) {
        addMessage('error', 'Please enter a WebSocket URL');
        return;
    }

    updateStatus('connecting');
    ws = new WebSocket(url);

    ws.onopen = () => {
        updateStatus('connected');
        connectBtn.disabled = true;
        disconnectBtn.disabled = false;
        messageInput.disabled = false;
        sendBtn.disabled = false;
        wsUrlInput.disabled = true;
        addMessage('received', 'Connected to server');
    };

    ws.onmessage = (event) => {
        try {
            const data = JSON.parse(event.data);
            if (data.echo !== undefined) {
                addMessage('received', `Echo: ${data.echo}`);
            } else {
                addMessage('received', event.data);
            }
        } catch (e) {
            addMessage('received', event.data);
        }
    };

    ws.onerror = (error) => {
        addMessage('error', 'WebSocket error occurred');
        console.error('WebSocket error:', error);
    };

    ws.onclose = () => {
        updateStatus('disconnected');
        connectBtn.disabled = false;
        disconnectBtn.disabled = true;
        messageInput.disabled = true;
        sendBtn.disabled = true;
        wsUrlInput.disabled = false;
        addMessage('received', 'Disconnected from server');
        ws = null;
    };
}

function disconnect() {
    if (ws) {
        ws.close();
    }
}

function sendMessage() {
    if (!ws || ws.readyState !== WebSocket.OPEN) {
        addMessage('error', 'Not connected to server');
        return;
    }

    const message = messageInput.value.trim();
    if (!message) {
        addMessage('error', 'Please enter a message');
        return;
    }

    // Send message with trailing newline as required by the server
    ws.send(message + '\n');
    addMessage('sent', message);
    messageInput.value = '';
}

connectBtn.addEventListener('click', connect);
disconnectBtn.addEventListener('click', disconnect);
sendBtn.addEventListener('click', sendMessage);

messageInput.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') {
        sendMessage();
    }
});
