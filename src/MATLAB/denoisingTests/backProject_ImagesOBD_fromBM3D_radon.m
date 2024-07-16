
testImages = filteredImagesBM3D;

nProjections =length(testImages);

step=(length(testImages))/nProjections;

angleStep=360.0/(nProjections+1);

s = size(testImages{2});


sinoGram = zeros([s(2) nProjections]);

for I=250:nProjections
    sI=round( (I-1)*step+1)
    sI2 = mod(round(sI+ length(testImages)/2-1),length(testImages))+1;
    
    projection =doOBDFilter( testImages{sI}, testImages{sI2});
    
    
    filteredImagesBM3D_OBD{I}=projection;
    
%     s2=size(projection);
%     startI=round( (size(projection,2) - s(2))/2);
%     
%     projection = projection(startI:end-startI,round(s2(2)/2))';
%     sinoGram(:,I)=projection(:);
end

% thetas = ((1:nProjections)-1)*angleStep;
% 
% reconImage=iradon(sinoGram,thetas,'linear','Ram-Lak',1,s(2));
% 
% figure;imagesc(reconImage(70:end-70,70:end-70));colormap gray;
% D=reconImage;


