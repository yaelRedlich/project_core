const uri = '/User';
const token = sessionStorage.getItem('token'); 
let users = []; 

const isTokenValid = (token) => {
    if (!token) 
        return false; 
    try {
        const payload = JSON.parse(atob(token.split('.')[1])); 
        const expiry = payload.exp * 1000; 
        return expiry > Date.now(); 
    } catch (error) {
        return false;
    }
}


const showUserName =(id) => {
    const userName = document.getElementById('userName');
    fetch(`${uri}/${id}`,{
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json'
        }
    })
    .then(response=>{
        if (!response.ok) {
            throw new Error("הגישה נדחתה! ייתכן שהמשתמש אינו מחובר.");
        }
        return response.json();
    })
    .then(data => {
        userName.innerHTML=data.username});
}
if (token) {
    if(!isTokenValid(token))
        location.href="index.html";
    const payload = token.split('.')[1];
    const decodedPayload = atob(payload);
    const jsonPayload = JSON.parse(decodedPayload);
    const userId = parseInt(jsonPayload.UserId, 10);
    jsonPayload.UserId=userId;
    showUserName(userId);
}
else {
    location.href="index.html";
}
const getItems = () => {
    fetch(uri, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("הגישה נדחתה! ייתכן שהמשתמש אינו מחובר.");
            }
            return response.json();
        })
        .then(data => _displayItems(data)) 
        .catch(error => alert(error.message));
        
}

const addItem = () =>{
    const addUsernameTextbox = document.getElementById('add-username');
    const addPasswordTextbox = document.getElementById('add-password');
    const addRoleCheckbox = document.getElementById('add-role');

    const item = {
        username: addUsernameTextbox.value.trim(),
        password: addPasswordTextbox.value.trim(),
        isAdmin: addRoleCheckbox.checked
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(() => {
            getItems();
            addUsernameTextbox.value = '';
            addPasswordTextbox.value = '';
            addRoleCheckbox.checked = false;
        })
        .catch(error => console.error('Unable to add item.', error));
}


const displayEditForm = (id) =>{    
    if(!isTokenValid(token))
        location.href="index.html";    
    const user = users.find(user => user.userId === id);
    if (!user) {
        console.error('User not found');
        return;
    }    
    document.getElementById('edit-id').value = user.userId;
    document.getElementById('edit-name').value = user.username;
    document.getElementById('edit-password').value = user.password;
    document.getElementById('edit-role').checked = user.isAdmin;
    document.getElementById('editForm').style.display = 'block';
}

const updateItem =() => {
    const userId = document.getElementById('edit-id').value;
    const userName = document.getElementById('edit-name').value.trim();
    const password = document.getElementById('edit-password').value.trim();
    const isAdmin = document.getElementById('edit-role').checked;

    const user = {
        userId : userId,
        username: userName,
        password: password,
        isAdmin: isAdmin
    };
    fetch(`${uri}/${userId}`, {
        method: 'PUT',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
    })
    .then(() => {
        getItems();
        document.getElementById('editForm').style.display = 'none';
    })
    .catch(error => console.error('Error updating user.', error));
}


const deleteItem = (id) =>{
    if(!isTokenValid(token))
        location.href="index.html";
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

const _displayItems = (data) => {
    const tBody = document.getElementById('users');
    tBody.innerHTML = '';
    const button = document.createElement('button');
    data.forEach(item => {
        let role = document.createElement('label');
        role.textContent = item.isAdmin ? 'Admin' : 'User';
        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.userId})`);
        let updataButton = button.cloneNode(false);
        updataButton.innerHTML='update';
        updataButton.setAttribute('onclick',`displayEditForm(${item.userId})`)
        let tr = tBody.insertRow();
        let td1 = tr.insertCell(0);
        td1.appendChild(document.createTextNode(item.userId));
        let td2 = tr.insertCell(1);
        td2.appendChild(document.createTextNode(item.username));
        let td3 = tr.insertCell(2);
        td3.appendChild(document.createTextNode(item.password));
        let td4 = tr.insertCell(3);
        td4.appendChild(role);
        let td5 = tr.insertCell(4);
        td5.appendChild(deleteButton);
        let td6 = tr.insertCell(5);
        td6.appendChild(updataButton);
    });
    users = data;
    
}
getItems();

